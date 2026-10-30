using System;
using System.Collections.Generic;

namespace InvoiceManagement.Models;

public partial class Purchase
{
    public int PurchaseId { get; set; }

    public DateTime PurchaseDate { get; set; }

    public int CompanyId { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();
}
