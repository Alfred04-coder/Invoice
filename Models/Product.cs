using System;
using System.Collections.Generic;

namespace InvoiceManagement.Models;

public partial class Product
{
    public int ProductId { get; set; }
    public string Name { get; set; }

    public decimal CostPrice { get; set; }

    public decimal SellingPrice { get; set; }

    public int QuantityInStock { get; set; }

    public string Description { get; set; } = null!;

    public int CompanyId { get; set; }

    public virtual Company? Company { get; set; } = null!;

    public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();

    public virtual ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();

    public virtual ICollection<StockTransaction> StockTransactions { get; set; } = new List<StockTransaction>();
}
