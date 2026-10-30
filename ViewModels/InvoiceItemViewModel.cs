namespace InvoiceManagement.ViewModels
{
    public class InvoiceItemViewModel
    {
        public int InvoiceItemId { get; set; }
        public string ProductName { get; set; }


        public int Quantity { get; set; }

        public decimal SellingPrice { get; set; }
        public decimal TotalPrice { get; set; }

        public int InvoiceId { get; set; }

        public int ProductId { get; set; }


    }
}
