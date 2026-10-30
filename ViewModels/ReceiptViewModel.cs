 

namespace InvoiceManagement.ViewModel
{
    public class ReceiptViewModel
    {
        public int PaymentId { get; set; }

        public string ReceiptNumber { get; set; } = "";

        public string InvoiceNumber { get; set; } = "";

        public DateTime PaymentDate { get; set; }

        public string CustomerName { get; set; } = "";

        public string CustomerPhone { get; set; } = "";

        public string CompanyName { get; set; } = "";

        public string CompanyPhone { get; set; } = "";

        public string CompanyAddress { get; set; } = "";

        public string? CompanyLogo { get; set; }

        public decimal GrandTotal { get; set; }

        public decimal AmountPaid { get; set; }

        public decimal BalanceAfterPayment { get; set; }

        public string PaymentMethod { get; set; } = "";
 
        public string? Reference { get; set; }
 
        public DateTime InvoiceDate { get; set; }
 
        public decimal InvoiceTotal { get; set; }
        public decimal AmountReceived { get; set; }
        public decimal TotalPaid { get; set; }
 
    }
}