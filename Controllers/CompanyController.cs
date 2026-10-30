using InvoiceManagement.Models.Data.InvoiceDbContext;
using InvoiceManagement.ViewModels;
using InvoiceManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

namespace InvoiceManagement.Controllers
{
    public class CompanyController : Controller
    {
        private readonly InvoiceDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public CompanyController(
            InvoiceDbContext context,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }
        //Get all Companies
        public async Task<IActionResult> Index()
        {
            var companies = await _context.Companies.ToListAsync();
            return View(companies);
        }

        //get Create Company 
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CompanyViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

 
        public async Task<IActionResult> Create(CompanyViewModel model)
        {
            // Validate form
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string? logoPath = null;

            // Upload company logo
            try
            {
                logoPath = await SaveCompanyLogoAsync(model.LogoFile);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(
                    nameof(model.LogoFile),
                    ex.Message
                );

                return View(model);
            }

            // Start database transaction
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                // Create company
                var company = new Company
                {
                    CompanyName = model.CompanyName,
                    Email = model.Email,
                    Phone = model.Phone,
                    Address = model.Address,
                    LogoPath = logoPath,
                    IsActive = true
                };

                _context.Companies.Add(company);

                await _context.SaveChangesAsync();

                // Create company administrator
                var admin = new ApplicationUser
                {
                    UserName = model.AdminEmail,
                    Email = model.AdminEmail,
                    CompanyId = company.CompanyId
                };

                var result = await _userManager.CreateAsync(
                    admin,
                    model.Password
                );

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(
                            "",
                            error.Description
                        );
                    }

                    await transaction.RollbackAsync();

                    return View(model);
                }

                // Assign Admin role
                var roleResult = await _userManager.AddToRoleAsync(
                    admin,
                    "Admin"
                );

                if (!roleResult.Succeeded)
                {
                    foreach (var error in roleResult.Errors)
                    {
                        ModelState.AddModelError(
                            "",
                            error.Description
                        );
                    }

                    await transaction.RollbackAsync();

                    return View(model);
                }

                // Save all changes
                await transaction.CommitAsync();

                TempData["SuccessMessage"] =
                    "Company and administrator created successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();

                ModelState.AddModelError(
                    "",
                    "An error occurred while creating the company. Please try again."
                );

                return View(model);
            }
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            var company = _context.Companies.Find(id);

            if (company == null)
                return NotFound();

            var vm = new CompanyViewModel
            {
                CompanyId = company.CompanyId,
                CompanyName = company.CompanyName,
                Email = company.Email,
                Phone = company.Phone,
                Address = company.Address
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CompanyViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var company =
                await _context.Companies.FindAsync(model.CompanyId);

            if (company == null)
                return NotFound();

            company.CompanyName = model.CompanyName;
            company.Email = model.Email;
            company.Phone = model.Phone;
            company.Address = model.Address;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ToggleStatus(int id)
        {
            var company = await _context.Companies.FindAsync(id);

            if (company == null)
                return NotFound();

            company.IsActive = !company.IsActive;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        private async Task<string?> SaveCompanyLogoAsync(IFormFile? logoFile)
        {
            if (logoFile == null || logoFile.Length == 0)
                return null;

            const long maxSize = 2 * 1024 * 1024;

            if (logoFile.Length > maxSize)
                throw new InvalidOperationException(
                    "Company logo must not exceed 2 MB.");

            var extension = Path.GetExtension(logoFile.FileName)
                .ToLowerInvariant();

            var allowedExtensions = new[]
            {
        ".jpg", ".jpeg", ".png", ".webp"
    };

            if (!allowedExtensions.Contains(extension))
                throw new InvalidOperationException(
                    "Only JPG, PNG and WEBP images are allowed.");

            var header = new byte[12];

            using (var stream = logoFile.OpenReadStream())
            {
                var read = await stream.ReadAsync(header, 0, header.Length);

                bool isPng = read >= 8 &&
                    header.Take(8).SequenceEqual(
                        new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 });

                bool isJpeg = read >= 3 &&
                    header[0] == 0xFF &&
                    header[1] == 0xD8 &&
                    header[2] == 0xFF;

                bool isWebp = read >= 12 &&
                    System.Text.Encoding.ASCII.GetString(header, 0, 4) == "RIFF" &&
                    System.Text.Encoding.ASCII.GetString(header, 8, 4) == "WEBP";

                bool valid = extension switch
                {
                    ".png" => isPng,
                    ".jpg" or ".jpeg" => isJpeg,
                    ".webp" => isWebp,
                    _ => false
                };

                if (!valid)
                    throw new InvalidOperationException(
                        "The uploaded file is not a valid supported image.");
            }

            var webRoot = _environment.WebRootPath
                ?? Path.Combine(_environment.ContentRootPath, "wwwroot");

            var folder = Path.Combine(
                webRoot, "uploads", "company-logos");

            Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid():N}{extension}";
            var filePath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(
                filePath, FileMode.CreateNew))
            {
                await logoFile.CopyToAsync(stream);
            }

            return $"/uploads/company-logos/{fileName}";
        }

    }
}
    

