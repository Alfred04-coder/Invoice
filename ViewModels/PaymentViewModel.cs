namespace InvoiceManagement.ViewModels
{
    public class PaymentViewModel
    {
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Method { get; set; }
        public bool ApplyTax { get; set; } = false;

        public string TaxType { get; set; } = "Percentage";

        public decimal TaxValue { get; set; } = 0;

        public string? ReceiptNumber { get; set; }
 
        public string? Reference { get; set; }
        public decimal BalanceAfterPayment { get; set; }

    }
}
