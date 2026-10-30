using InvoiceManagement.Models;
using InvoiceManagement.Models.Data.InvoiceDbContext;
using InvoiceManagement.ViewModel;
using InvoiceManagement.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
 

namespace InvoiceManagement.Controllers
{
    public class StaffController : Controller
    {
        private readonly InvoiceDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public StaffController(InvoiceDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        [HttpGet]
        public async Task <IActionResult> Create()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || currentUser.CompanyId == null)
                return Unauthorized();
            var companyId = currentUser.CompanyId.Value;
            var vm = new InvoiceViewModel
            {
                Products = _context.Products.Where(p => p.CompanyId == companyId).Select(p => new ProductViewModel 
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    CostPrice = p.CostPrice,
                    SellingPrice = p.SellingPrice,
                    QuantityInStock = p.QuantityInStock,
                    Description = p.Description,
                    CompanyId = p.CompanyId,
                }).ToList(),
                Items = new List<InvoiceItemViewModel>
                {
                    new InvoiceItemViewModel(),
                    new InvoiceItemViewModel()
                }
            };
            return View(vm);
        }


        public async Task<IActionResult> Create(InvoiceViewModel vm)
        {
            using var dbTran = _context.Database.BeginTransaction();
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }
            var companyId = currentUser.CompanyId;
            ModelState.Remove(nameof(Product.CompanyId));


            try
            {

                var customer = _context.Customers.FirstOrDefault(c => c.CompanyId == companyId);

                if (customer == null)
                {
                    customer = new Customer
                    {
                        CustomerName = vm.CustomerName,
                        CompanyId = (int)companyId,
                        CreatedDate = DateTime.Now,
                        Phone = vm.CustomerPhone ?? "",
                        Address = vm.Address ?? ""
                    };

                    _context.Customers.Add(customer);
                    _context.SaveChanges();
                }
                var invoice = new Models.Invoice
                {
                    InvoiceDate = DateTime.Today,
                    CompanyId = (int)companyId,
                    CustomerId = customer.CustomerId,
                    CustomerName = vm.CustomerName,
                    CustomerPhone = vm.CustomerPhone ?? "",
                    IsCredit = vm.IsCredit,
                    InvoiceItems = new List<InvoiceItem>(),
                                        Status = "Issued"

                };

                if (vm.Items == null || !vm.Items.Any(
                        x => x.ProductId > 0 && x.Quantity > 0))
                {
                    throw new Exception(
                        "No valid invoice items were received. " +
                        "Please select a product and enter a quantity.");
                }

                foreach (var item in vm.Items.Where(
                             x => x.ProductId > 0 && x.Quantity > 0))
                {
                    var product = _context.Products.FirstOrDefault(p =>
                        p.ProductId == item.ProductId &&
                        p.CompanyId == companyId);

                    if (product == null)
                    {
                        throw new Exception("Product not found for this company.");
                    }

                    if (product.QuantityInStock < item.Quantity)
                    {
                        throw new Exception(
                            $"Insufficient stock for {product.Name}.");
                    }

                    product.QuantityInStock -= item.Quantity;


                    decimal SellingPrice =
                        item.SellingPrice > 0 ? item.SellingPrice : product.SellingPrice;


                    if (SellingPrice <= 0)
                    {
                        throw new Exception(
                            $"Invalid selling price for {product.Name}.");
                    }

                    var invoiceItem = new InvoiceItem
                    {
                        ProductId = product.ProductId,
                        Quantity = item.Quantity,


                        SellingPrice = SellingPrice
                    };

                    invoice.InvoiceItems.Add(invoiceItem);

                    _context.StockTransactions.Add(new StockTransaction
                    {
                        CompanyId = (int)companyId,
                        ProductId = product.ProductId,
                        Quantity = item.Quantity,
                        Type = "OUT",
                        Date = DateTime.Now
                    });
                }

                // CALCULATE GRAND TOTAL, AMOUNT PAID, AND BALANCE

                invoice.Subtotal = invoice.InvoiceItems.Sum(
                    x => x.Quantity * x.SellingPrice);

                // D TAX'S
                invoice.TaxAmount = 0;
                invoice.TaxValue = 0;
                invoice.TaxType = null;

                // Apply tax only when selected
                if (vm.ApplyTax)
                {
                    if (vm.TaxValue < 0)
                    {
                        throw new Exception("Tax value cannot be negative.");
                    }

                    if (vm.TaxType != "Percentage" &&
                        vm.TaxType != "Fixed")
                    {
                        throw new Exception("Invalid tax type.");
                    }

                    if (vm.TaxType == "Percentage" && vm.TaxValue > 100)
                    {
                        throw new Exception(
                            "Tax percentage cannot exceed 100%.");
                    }

                    invoice.TaxType = vm.TaxType;
                    invoice.TaxValue = vm.TaxValue;

                    if (vm.TaxType == "Percentage")
                    {
                        invoice.TaxAmount = Math.Round(
                            invoice.Subtotal * vm.TaxValue / 100,
                            2,
                            MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        invoice.TaxAmount = vm.TaxValue;
                    }
                }

                // TOTAL INVOICE
                invoice.GrandTotal =
                    invoice.Subtotal + invoice.TaxAmount;

                // Payment calculations
                invoice.AmountPaid = vm.IsCredit
                    ? vm.AmountPaid
                    : invoice.GrandTotal;

                if (invoice.AmountPaid < 0 ||
                    invoice.AmountPaid > invoice.GrandTotal)
                {
                    throw new Exception("Invalid amount paid.");
                }

                invoice.Balance =
                    invoice.GrandTotal - invoice.AmountPaid;

                // ADD INVOICE 

                // SAVE INVOICE AND INVOICE ITEMS
                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync();

                // SAVE THE INITIAL PAYMENT, IF ANY MONEY WAS RECEIVED
                if (invoice.AmountPaid > 0)
                {
                    var payment = new Payment
                    {
                        InvoiceId = invoice.InvoiceId,
                        Amount = invoice.AmountPaid,
                        PaymentDate = DateTime.Now,
                        Method = "Cash",
                        Reference = "",
                        BalanceAfterPayment = invoice.Balance
                    };

                    _context.Payments.Add(payment);
                    await _context.SaveChangesAsync();

                    // Generate a receipt number using the saved PaymentId.
                    payment.ReceiptNumber =
                        $"RCT-{payment.PaymentId:D6}";

                    await _context.SaveChangesAsync();
                }

                // SAVE EVERYTHING BEFORE COMMITTING
                await dbTran.CommitAsync();

                // If a payment was received, open its receipt.
                if (invoice.AmountPaid > 0)
                {
                    var savedPaymentId = await _context.Payments
                        .Where(p => p.InvoiceId == invoice.InvoiceId)
                        .OrderByDescending(p => p.PaymentId)
                        .Select(p => p.PaymentId)
                        .FirstAsync();

                    return RedirectToAction(
                        "ViewReceipt",
                        new { id = savedPaymentId });
                }

                // No payment was received: show the invoice.
                return RedirectToAction(
                    "ViewInvoice",
                    new { id = invoice.InvoiceId });

 
            }
            catch (Exception ex)
            {
                dbTran.Rollback();
                ModelState.AddModelError("", ex.Message);

                vm.Products = _context.Products.Select(p => new ProductViewModel
                {
                    Name = p.Name,
                    ProductId = p.ProductId,
                    QuantityInStock = p.QuantityInStock,
                    SellingPrice = p.SellingPrice
                }).ToList();

                return View(vm);
            }
        }



        [HttpGet]
        public async Task<IActionResult> InvoiceResult(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            var invoice = await _context.Invoices
                .AsNoTracking()
                .Where(x => x.InvoiceId == id
                         && x.CompanyId == user.CompanyId
                         && !x.IsDeleted)
                .Select(x => new InvoiceViewModel
                {
                    InvoiceId = x.InvoiceId,
                    InvoiceNumber = "INV-" + x.InvoiceId,
                    CompanyName = x.Company.CompanyName,
                    CompanyPhone = x.Company.Phone,
                    Address = x.Company.Address,
                    CompanyLogo = x.Company.LogoPath,

                    InvoiceDate = x.InvoiceDate,
                    CustomerName = x.CustomerName,
                    CustomerPhone = x.CustomerPhone,

                    Items = x.InvoiceItems.Select(i =>
                        new InvoiceItemViewModel
                        {
                            ProductName = i.Product.Name,
                            Quantity = i.Quantity,
                            SellingPrice = i.SellingPrice,
                            TotalPrice = i.TotalPrice
                        }).ToList(),

                    Payments = x.Payments
                        .OrderBy(p => p.PaymentDate)
                        .ThenBy(p => p.PaymentId)
                        .Select(p => new PaymentViewModel
                        {
                            PaymentId = p.PaymentId,
                            ReceiptNumber = p.ReceiptNumber,
                            Amount = p.Amount,
                            PaymentDate = p.PaymentDate,
                            Method = p.Method,
                            Reference = p.Reference,
                            BalanceAfterPayment = p.BalanceAfterPayment
                        }).ToList(),

                    GrandTotal = x.GrandTotal,
                    AmountPaid = x.AmountPaid,
                    Balance = x.Balance,
                    IsCredit = x.IsCredit,
                    Subtotal = x.Subtotal,
                    TaxAmount = x.TaxAmount,
                    TaxType = x.TaxType,
                    TaxValue = x.TaxValue
                })
                .FirstOrDefaultAsync();

            if (invoice == null)
                return NotFound();

            return View("InvoiceResult", invoice);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecordPayment(int invoiceId, decimal amount, string method, string? reference)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            if (amount <= 0)
                return BadRequest("Payment must be greater than zero.");

            var allowedMethods = new[]
            {
        "Cash", "MoMo", "Bank"
            };

            if (!allowedMethods.Contains(method))
                return BadRequest("Invalid payment method.");

            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    System.Data.IsolationLevel.Serializable);

            var invoice = await _context.Invoices
                .FirstOrDefaultAsync(i =>
                    i.InvoiceId == invoiceId &&
                    i.CompanyId == user.CompanyId &&
                    !i.IsDeleted);

            if (invoice == null)
                return NotFound();

            if (invoice.Balance <= 0)
                return BadRequest("Invoice is already fully paid.");

            if (amount > invoice.Balance)
                return BadRequest("Payment exceeds outstanding balance.");

            invoice.AmountPaid += amount;
            invoice.Balance = invoice.GrandTotal - invoice.AmountPaid;

            invoice.PaymentStatus = invoice.Balance <= 0
                ? "Paid"
                : invoice.AmountPaid > 0
                    ? "Partially Paid"
                    : "Unpaid";

            var payment = new Payment
            {
                InvoiceId = invoice.InvoiceId,
                Amount = amount,
                PaymentDate = DateTime.UtcNow,
                Method = method,
                Reference = reference ?? "",
                BalanceAfterPayment = invoice.Balance
            };

            _context.Payments.Add(payment);

            await _context.SaveChangesAsync();

            // Use the database-generated PaymentId for a unique receipt.
            payment.ReceiptNumber =
                $"RCT-{payment.PaymentId:D6}";

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return RedirectToAction(
                "ViewReceipt",
                new { id = payment.PaymentId });
        }






        [HttpGet]
        public async Task<IActionResult> ViewReceipt(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            var receipt = await _context.Payments
                .AsNoTracking()
                .Where(p => p.PaymentId == id
                         && p.Invoice.CompanyId == user.CompanyId
                         && !p.Invoice.IsDeleted)
                .Select(p => new ReceiptViewModel
                {
                    PaymentId = p.PaymentId,
                    ReceiptNumber = p.ReceiptNumber ?? $"RCT-{p.PaymentId:D6}",
                    InvoiceNumber = "INV-" + p.Invoice.InvoiceId,
                    InvoiceDate = p.Invoice.InvoiceDate,
                    PaymentDate = p.PaymentDate,

                    CompanyName = p.Invoice.Company.CompanyName,
                    CompanyPhone = p.Invoice.Company.Phone,
                    CompanyAddress = p.Invoice.Company.Address,
                    CompanyLogo = p.Invoice.Company.LogoPath,

                    CustomerName = p.Invoice.CustomerName,
                    CustomerPhone = p.Invoice.CustomerPhone,

                    InvoiceTotal = p.Invoice.GrandTotal,
                    AmountReceived = p.Amount,
                    TotalPaid = p.Invoice.AmountPaid,
                    BalanceAfterPayment = p.BalanceAfterPayment,

                    PaymentMethod = p.Method,
                    Reference = p.Reference
                })
                .FirstOrDefaultAsync();

            if (receipt == null)
                return NotFound();

            return View("ViewReceipt", receipt);
        }

    }
}
