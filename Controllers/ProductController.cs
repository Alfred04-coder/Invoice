using InvoiceManagement.Models.Data.InvoiceDbContext;
using InvoiceManagement.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using InvoiceManagement.Models;
namespace InvoiceManagement.Controllers
{
    public class ProductController : Controller
    {
        private readonly InvoiceDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductController(InvoiceDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index(string searchString)
        {
            var currentUaer = await _userManager.GetUserAsync(User);
            if (currentUaer == null)
            {
                return Unauthorized();
            }
            var caponayId = currentUaer.CompanyId;
            var prodcts = _context.Products.Where(p => p.CompanyId == caponayId).AsQueryable();
            if (!string.IsNullOrEmpty(searchString))
            {
                prodcts = prodcts.Where(p => p.Name.Contains(searchString));
            }
            return View(prodcts.ToList());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }
            var caponayId = currentUser.CompanyId;
            ModelState.Remove(nameof(Product.Company));

            if (product.SellingPrice < product.CostPrice)
            {
                ModelState.AddModelError("SellingPrice", "Selling price cannot be less than cost price.");
                return View(product);
            }
            if (!ModelState.IsValid)
            {
                return View(product);
            }


            product.CompanyId = (int)caponayId;
            _context.Products.Add(product);
            _context.StockTransactions.Add(new StockTransaction
            {
                CompanyId = (int)currentUser.CompanyId,
                Product = product,
                Quantity = product.QuantityInStock,
                Type = "IN",
                Date = DateTime.Now,
            });

            Console.WriteLine($"Logged in user: {currentUser.Email}");
            Console.WriteLine($"CompanyId: {currentUser.CompanyId}");
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> AddStock(int productId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var companyId = currentUser.CompanyId;
            var product = _context.Products.FirstOrDefault(p => p.ProductId == productId && p.CompanyId == companyId);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> AddStock(int productId, int quantity)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var companyId = currentUser.CompanyId;
            var product = _context.Products.FirstOrDefault(p => p.ProductId == productId && p.CompanyId == companyId);
            if (product == null)
            {
                return NotFound();
            }
            if (quantity <= 0)
            {
                ModelState.AddModelError("Quantity", "Quantity to add must be greater than zero.");
                return View(product);
            }
            product.QuantityInStock += quantity;
            _context.StockTransactions.Add(new StockTransaction
            {
                ProductId = productId,
                CompanyId = currentUser.CompanyId.Value,
                Product = product,
                Quantity = quantity,
                Type = "IN",
                Date = DateTime.Now,
            });
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int ProductId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var companyId = currentUser.CompanyId;
            var product = _context.Products.FirstOrDefault(p => p.ProductId == ProductId && p.CompanyId == companyId);
            var ProdudctViewModel = new ProductViewModel
            {
                CompanyId = (int)currentUser.CompanyId,
                ProductId = ProductId,
                Name = product.Name,
                CostPrice = product.CostPrice,
                SellingPrice = product.SellingPrice,
                QuantityInStock = product.QuantityInStock,
                Description = product.Description,

            };
            return View(ProdudctViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductViewModel model)
        {
            var currentUser = await _userManager.GetUserAsync (User);
            var CompanyId = currentUser.CompanyId;

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            ModelState.Remove(nameof(Product.Company));
            var product = _context.Products.FirstOrDefault(p => p.ProductId == model.ProductId && p.CompanyId == CompanyId);
            if (product == null) {return View(model);}

            if (product.CostPrice > product.SellingPrice)
            {
                ModelState.AddModelError("","Selling Price Can not be less than Cost Price");
                return View(model);
            }
            var qtyDelta = (int)(model.QuantityInStock - product.QuantityInStock);

 
            product.Name = model.Name;
            product.CostPrice = model.CostPrice;
            product.SellingPrice = model.SellingPrice;
            product.QuantityInStock = model.QuantityInStock;
            product.Description = model.Description;
            //product.ExpiryDate = model.ExpiryDate;





           await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));

        }

        public IActionResult StockSummary(string filter, DateTime? startDate, DateTime? endDate)
        {

            var currentUser = _userManager.GetUserAsync(User);
            var companyId = currentUser.Result.CompanyId;

            DateTime start;
            DateTime end = DateTime.Today;

            switch (filter)
            {
                case "today":
                    start = DateTime.Today;
                    end = DateTime.Today;
                    break;

                case "yesterday":
                    start = DateTime.Today.AddDays(-1);
                    end = start;
                    break;

                case "3days":
                    start = DateTime.Today.AddDays(-3);
                    break;

                case "thisweek":
                    int diff = (7 + (DateTime.Today.DayOfWeek - DayOfWeek.Monday)) % 7;
                    start = DateTime.Today.AddDays(-diff);
                    break;

                case "lastweek":
                    int diffLast = (7 + (DateTime.Today.DayOfWeek - DayOfWeek.Monday)) % 7;
                    end = DateTime.Today.AddDays(-diffLast - 1);
                    start = end.AddDays(-6);
                    break;

                default:
                    end = endDate ?? DateTime.Today;
                    start = startDate ?? end.AddDays(-6);
                    break;
            }

            var summary = _context.StockTransactions
                .Where(t => t.Type == "OUT" && t.CompanyId == companyId
                            && t.Date.Date >= start
                            && t.Date.Date <= end)
                .GroupBy(t => new { t.ProductId, t.Product.Name })
                .Select(g => new StockSummaryViewModel
                {
                    ProductName = g.Key.Name,
                    QuantitySold = g.Sum(t => t.Quantity)
                })
                .OrderByDescending(s => s.QuantitySold)
                .ToList();

            ViewBag.TotalSold = summary.Sum(x => x.QuantitySold);
            ViewBag.StartDate = start;
            ViewBag.EndDate = end;
            ViewBag.Filter = filter;

            return View(summary);
        }
        public IActionResult DailyStockSales()
        {
            var sales = _context.StockTransactions
                .Where(t => t.Type == "OUT")
                .GroupBy(t => new
                {
                    t.ProductId,
                    Date = t.Date.Date
                })
                .Select(g => new DailyProductSalesViewModel
                {
                    ProductName = g.First().Product.Name,
                    Date = g.Key.Date,
                    QuantitySold = g.Sum(x => x.Quantity)
                })
                .OrderBy(x => x.ProductName)
                .ThenByDescending(x => x.Date)
                .ToList();



            return View(sales);
        }
    }

    

}
