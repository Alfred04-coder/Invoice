using InvoiceManagement.Models;
using InvoiceManagement.ViewModels;
 
using System.ComponentModel.DataAnnotations;

namespace InvoiceManagement.ViewModel
{
    public class InvoiceViewModel
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyPhone { get; set; }
        public string? CompanyLogo { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TaxAmount { get; set; }
        public bool ApplyTax { get; set; } = false;

        public string TaxType { get; set; } = "Percentage";

        public decimal TaxValue { get; set; } = 0;
        public string Address { get; set; }
        public bool IsCredit { get; set; }
        public decimal AmountPaid { get; set; }

        public decimal GrandTotal { get; set; }
        public decimal TotalBill { get; set; }
        public decimal OutstandingDebt { get; set; }
        public decimal Balance { get; set; }


        public List<InvoiceItemViewModel> Items { get; set; } = new();
        public List<ProductViewModel> Products { get; set; }
        public List<PaymentViewModel> Payments { get; set; }
    }

    public class InvoiceCreateViewModel
    {
        public List<Product> Products { get; set; } = new();
        public List<InvoiceItemViewModel> Items { get; set; } = new();
    }
    public class RecentInvoiceVM
    {
        public int CompanyId { get; set; }
        public int InvoiceId { get; set; }
        public object InvoiceNumber { get; set; }
        public string CustomerName { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal GrandTotal { get; set; }
    }


    public class MultiInvoiceViewModel
    {
        public List<InvoiceViewModel> Invoices { get; set; } = new List<InvoiceViewModel>();
        //public List<InvoiceTableViewModel> Tables { get; set; } = new();
        public List<Product> Products { get; set; } = new List<Product>();
    }


    public class InvoiceItemVM
    {
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class ProductDropdownVM
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public decimal QuantityInStock { get; set; }
    }

    //public class InvoiceViewModel
    //{
    //    [Required]
    //    public string CustomerName { get; set; }
    //    [Required]
    //    public string CustomerPhone { get; set; }
    //     public int InvoiceId { get; set; }
    //     public string InvoiceNumber { get; set; }
    //    public DateTime InvoiceDate { get; set; }
    //    public bool IsCredit { get; set; }
    //    public decimal AmountPaid { get; set; }

    //    public decimal GrandTotal { get; set; }
    //    public decimal TotalBill { get; set; }
    //    public decimal OutstandingDebt { get; set; }
    //    public decimal Balance { get; set; }


    //    public List<InvoiceItemViewModel> Items { get; set; } = new();
    //    public List<ProductViewModel> Products { get; set; } = new();
    //    //public List<PaymentViewModel> Payments { get; set; } = new List<PaymentViewModel>();
    //    public List<PaymentViewModel> Payments { get; set; }

    //}

    //public class MultiInvoiceViewModel
    //{
    //    public List<InvoiceViewModel> Invoices { get; set; } = new List<InvoiceViewModel>();
    //    public List<InvoiceTableViewModel> Tables { get; set; } = new();
    //    public List<Product> Products { get; set; } = new List<Product>();
    //}


    //public class InvoiceItemVM
    //{
    //    public int ProductId { get; set; }
    //    public decimal Quantity { get; set; }
    //    public decimal UnitPrice { get; set; }
    //}

    //public class ProductDropdownVM
    //{
    //    public int ProductId { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //    public decimal UnitPrice { get; set; }
    //    public decimal QuantityInStock { get; set; }
    //}




    //public class InvoiceViewModel
    //{
    //    public string CustomerName { get; set; }
    //    public string CustomerPhone { get; set; }

    //    public int InvoiceId { get; set; }
    //    public string InvoiceNumber { get; set; }
    //    public DateTime InvoiceDate { get; set; }


    //    public List<InvoiceItemViewModel> Items { get; set; } = new();
    //    public List<InvoiceTableViewModel> Tables { get; set; } = new();
    //    public List<ProductViewModel> Products { get; set; }

    //    // Used for dropdown
    //    //public List<ProductDropdownVM> Products { get; set; } = new();


    //    //public class InvoiceItemVM
    //    //{
    //    //    public int ProductId { get; set; }
    //    //    public decimal Quantity { get; set; }
    //    //    public decimal UnitPrice { get; set; }
    //    //}



    //}


    //public class ProductDropdownVM
    //{
    //    public int ProductId { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //    public decimal UnitPrice { get; set; }
    //    public decimal QuantityInStock { get; set; }
    //}

}
