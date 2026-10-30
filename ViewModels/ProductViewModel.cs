namespace InvoiceManagement.ViewModels
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }
        
        public string Name { get; set; }

        public decimal CostPrice { get; set; }

        public decimal SellingPrice { get; set; }

        public int QuantityInStock { get; set; }

        public string Description { get; set; } = null!;

        public int CompanyId { get; set; }
    }
}
