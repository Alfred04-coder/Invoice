using InvoiceManagement.Models;
using InvoiceManagement.Models.Data.InvoiceDbContext;
using InvoiceManagement.ViewModel;
using InvoiceManagement.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InvoiceManagement.Controllers
{
    public class AdminController : Controller
    {
        private readonly InvoiceDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public AdminController(InvoiceDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Dashboard(string filter = "month")
        {
            var currentUserr = await _userManager.GetUserAsync(User);
            var companyId = currentUserr.CompanyId;
            var today = DateTime.Today;
            var invoicesQuery = _context.Invoices.AsQueryable();

            invoicesQuery = filter switch
            {
                "today" => invoicesQuery.Where(i => i.InvoiceDate.Date == today),
                "year" => invoicesQuery.Where(i => i.InvoiceDate.Year == today.Year),
                _ => invoicesQuery.Where(i =>
                    i.InvoiceDate.Month == today.Month &&
                    i.InvoiceDate.Year == today.Year)
            };

            var invoices = invoicesQuery.ToList();

            var vm = new DashboardViewModel
            {
 
                SelectedFilter = filter,
                TotalSales = invoices.Where(i => i.CompanyId == companyId).Sum(i => i.GrandTotal ),
                TotalInvoices = invoices.Where(i => i.CompanyId == companyId).Count(),
                TotalProducts = _context.Products.Where(i => i.CompanyId == companyId).Count(),
                LowStockCount = _context.Products.Where(i => i.CompanyId == companyId).Count(p => p.QuantityInStock < 10),
                TotalDebt = _context.Invoices.Where(i => i.Balance > 0).Sum(i => i.Balance)
            };

            vm.RecentInvoices = _context.Invoices.Where(i => i.CompanyId == companyId)
                .OrderByDescending(i => i.InvoiceDate )
                .Take(5)
                .Select(i => new RecentInvoiceViewModel
                {
 
                    InvoiceId = i.InvoiceId,
                    //InvoiceNumber = i.InvoiceNumber,
                    CustomerName = i.CustomerName,
                    InvoiceDate = i.InvoiceDate,
                    GrandTotal = i.GrandTotal
                })
                .ToList();


            vm.TopCustomers = _context.Invoices.Where(i => i.CompanyId == companyId)
                .GroupBy(i => i.CustomerName)
                .Select(g => new TopCustomerVM
                {
                    CustomerName = g.Key,
                    InvoiceCount = g.Count(),
                    TotalSpent = g.Sum(x => x.GrandTotal)
                })
                .OrderByDescending(x => x.TotalSpent)
                .Take(5)
                .ToList();

            return View(vm);
        }


        [HttpGet]
        public IActionResult ViewInvoice(int id)
        {
            var invoice = _context.Invoices.Include(x => x.InvoiceItems).ThenInclude(i => i.Product).Include(p => p.Payments).Where(x => x.InvoiceId == id)
                .Select(x => new InvoiceViewModel
                {
                    InvoiceId = x.InvoiceId,
                    InvoiceNumber = "INV-" + x.InvoiceId,
                    CompanyName = x.Company.CompanyName,
                    CompanyPhone = x.Company.Phone,
                    Address = x.Company.Address,
                    CompanyLogo = x.Company.LogoPath,
                    InvoiceDate = DateTime.Now,
                    CustomerName = x.CustomerName,
                    CustomerPhone = x.CustomerPhone,



                    Items = x.InvoiceItems.Select(i => new InvoiceItemViewModel
                    {
                        ProductName = i.Product.Name,
                        Quantity = i.Quantity,
                        SellingPrice = i.SellingPrice,
                        TotalPrice = i.TotalPrice,
                    }).ToList(),

                    Payments = x.Payments != null ?
                    x.Payments.Select(p => new PaymentViewModel
                    {
                        Amount = p.Amount,
                        PaymentDate = DateTime.Now,
                        Method = p.Method,
                    }).ToList()
                    : new List<PaymentViewModel>(),

                    GrandTotal = x.GrandTotal,
                    AmountPaid = x.AmountPaid,
                    Balance = x.Balance,
                    IsCredit = x.IsCredit,
                    Subtotal = x.Subtotal,
                    TaxAmount = x.TaxAmount,
                    TaxType = x.TaxType,
                    TaxValue = x.TaxValue
                })
               .FirstOrDefault();

            if (invoice == null)
                return NotFound();

            return View("InvoiceResult", invoice);

        }


        public async Task<IActionResult> Debtors()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var companyId = currentUser.CompanyId;

            var model = _context.Invoices.Where(i => i.Balance > 0 && i.CompanyId == companyId).Select(i => new InvoiceViewModel
            {
                InvoiceId = i.InvoiceId,
                InvoiceNumber = "INV-" + i.InvoiceId,
                InvoiceDate = i.InvoiceDate,
                CustomerName = i.CustomerName,
                GrandTotal = i.GrandTotal,
                AmountPaid = i.AmountPaid,
                Balance = i.Balance
            }).ToList();
            return View(model);
        }


        public async Task<IActionResult> CustomerInvoices(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var companyId = currentUser.CompanyId;
            var invoicesQuery = _context.Invoices.AsQueryable();
            var invoices = _context.Invoices.Where(i => i.InvoiceId == id && i.CompanyId == companyId).Select(i => new InvoiceViewModel
            {
                InvoiceId = i.InvoiceId,
                CompanyId = i.CompanyId,
                InvoiceNumber = "INV-" + i.InvoiceId,
                CustomerName = i.CustomerName,
                CustomerPhone = i.CustomerPhone,
                InvoiceDate = DateTime.Now,
                GrandTotal = i.GrandTotal,
                AmountPaid = i.AmountPaid,
                Balance = i.Balance,




            }).ToList();
            return View(invoices);
        }

        public async Task<IActionResult> SalesList(string filter = "month", string search = "")
        {
            var currentUse = await _userManager.GetUserAsync(User);
            var companyId = currentUse.CompanyId;
            var today = DateTime.Today;
            var invoicesQuery = _context.Invoices
                .Where(i => i.CompanyId == companyId);

            // Apply date filter//
            invoicesQuery = filter.ToLower() switch
            {
                "today" => invoicesQuery.Where(i => i.InvoiceDate.Date == today),
                "yesterday" => invoicesQuery.Where(i => i.InvoiceDate.Date == today.AddDays(-1)),
                "last7" => invoicesQuery.Where(i => i.InvoiceDate >= today.AddDays(-7)),
                "last30" => invoicesQuery.Where(i => i.InvoiceDate >= today.AddDays(-30)),
                "month" => invoicesQuery.Where(i => i.InvoiceDate.Month == today.Month && i.InvoiceDate.Year == today.Year),
                "year" => invoicesQuery.Where(i => i.InvoiceDate.Year == today.Year),
                _ => invoicesQuery
            };

            // Apply search by customer name if provided
            if (!string.IsNullOrEmpty(search))
            {
                invoicesQuery = invoicesQuery.Where(i => i.CustomerName.Contains(search) && i.CompanyId == companyId);
            }

            // Project to view model
            var invoices = invoicesQuery
                .OrderByDescending(i => i.InvoiceId)
                .Select(i => new InvoiceListViewModel
                {
                    InvoiceId = i.InvoiceId,
                    InvoiceNumber = "INV-" + i.InvoiceId,
                    CustomerName = i.CustomerName,
                    CustomerPhone = i.CustomerPhone,
                    Date = i.InvoiceDate,
                    GrandTotal = i.GrandTotal
                })
                .ToList();

            // Total sales
            ViewBag.TotalSales = invoices.Sum(x => x.GrandTotal);

            // Pass search term and filter back to view for UI
            ViewBag.Search = search;
            ViewBag.Filter = filter;

            return View(invoices);
        }


        [HttpGet]
        public async Task<IActionResult> EditInvoice(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
                return Challenge();

            var companyId = currentUser.CompanyId;

            var invoice = await _context.Invoices
                .Where(i => i.InvoiceId == id && i.CompanyId == companyId)
                .Select(i => new InvoiceViewModel
                {
                    InvoiceId = i.InvoiceId,
                    CustomerName = i.CustomerName,
                    CustomerPhone = i.CustomerPhone,

                    Items = i.InvoiceItems.Select(item => new InvoiceItemViewModel
                    {
                        InvoiceId = item.InvoiceId,
                        InvoiceItemId = item.InvoiceItemId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        SellingPrice = item.SellingPrice
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (invoice == null)
                return NotFound();

            invoice.Products = await _context.Products
                .Where(p => p.CompanyId == companyId)
                .Select(p => new ProductViewModel
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    SellingPrice = p.SellingPrice
                })
                .ToListAsync();

            return View(invoice);
        }


        [HttpPost]

        public async  Task<IActionResult> EditInvoice(InvoiceViewModel vm)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var companyId = currentUser.CompanyId;
            using var tran = _context.Database.BeginTransaction();
            try
            {
                var invoice = _context.Invoices
                    .Include(i => i.InvoiceItems)
                    .Include(i => i.StockTransactions)
                    .Include(i => i.Payments)
                    .FirstOrDefault(i => i.InvoiceId == vm.InvoiceId && i.CompanyId == companyId);

                if (invoice == null) return NotFound();


                foreach (var oldItem in invoice.InvoiceItems.ToList())
                {
                    var product = _context.Products.Find(oldItem.ProductId);
                    if (product != null)
                    {
                        product.QuantityInStock += oldItem.Quantity;
                    }
                }

                var oldOutTransactions = _context.StockTransactions
                    .Where(t => t.InvoiceId == invoice.InvoiceId && t.Type == "OUT")
                    .ToList();

                if (oldOutTransactions.Any())
                    _context.StockTransactions.RemoveRange(oldOutTransactions);


                _context.InvoiceItems.RemoveRange(invoice.InvoiceItems);
                invoice.InvoiceItems.Clear();


                decimal grandTotal = 0m;

                foreach (var item in vm.Items.Where(x => x.ProductId > 0 && x.Quantity > 0))
                {
                    var product = _context.Products.Find(item.ProductId)
                                  ?? throw new Exception("Invalid product");

                    if (product.QuantityInStock < item.Quantity)
                        throw new Exception($"Not enough stock for {product.Name}. Available: {product.QuantityInStock}, requested: {item.Quantity}");


                    product.QuantityInStock -= item.Quantity;

                    var sellingPrice = item.SellingPrice > 0 ? item.SellingPrice : product.SellingPrice;

                    var invoiceItem = new InvoiceItem
                    {
                        ProductId = product.ProductId,
                        Quantity = item.Quantity,
                        SellingPrice = sellingPrice
                        
                    };
                    invoice.InvoiceItems.Add(invoiceItem);


                    _context.StockTransactions.Add(new StockTransaction
                    {
                        InvoiceId = invoice.InvoiceId,
                        ProductId = product.ProductId,
                        CompanyId = (int)companyId,
                        Quantity = item.Quantity,
                        Type = "OUT",

                        Date = DateTime.Now
                    });

                    grandTotal += sellingPrice * item.Quantity;
                }


                invoice.CustomerName = vm.CustomerName;
                invoice.CustomerPhone = vm.CustomerPhone;
                invoice.GrandTotal = grandTotal;


                invoice.AmountPaid = invoice.Payments?.Sum(p => p.Amount) ?? invoice.AmountPaid;
                invoice.Balance = invoice.GrandTotal - invoice.AmountPaid;
                invoice.IsCredit = invoice.Balance > 0;

                _context.SaveChanges();
                tran.Commit();

                return RedirectToAction("SalesList");
            }
            catch (Exception ex)
            {
                tran.Rollback();
                ModelState.AddModelError("", ex.Message);

                vm.Products = _context.Products
                    .Select(p => new ProductViewModel
                    {
                        ProductId = p.ProductId,
                        Name = p.Name,
                        SellingPrice = p.SellingPrice
                    }).ToList();

                return View(vm);
            }
        }

        public IActionResult StockSummary(string filter, DateTime? startDate, DateTime? endDate)
        {
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
                .Where(t => t.Type == "OUT"
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
    }
}
