namespace InvoiceManagement.ViewModel
{
    public class InvoiceListViewModel
    {
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public DateTime Date { get; set; }
        public decimal GrandTotal { get; set; }
    }
}
