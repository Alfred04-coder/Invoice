using System;
using System.Collections.Generic;

namespace InvoiceManagement.Models;

public partial class StockTransaction
{
    public int StockTransactionId { get; set; }

    public int Quantity { get; set; }

    public string Type { get; set; } = null!;
    public int? InvoiceId { get; set; }   // 👈 THIS

    public Invoice Invoice { get; set; }
    public DateTime Date { get; set; }

    public int CompanyId { get; set; }

    public int ProductId { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
