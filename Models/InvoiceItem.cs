using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvoiceManagement.Models;

public partial class InvoiceItem
{
    public int InvoiceItemId { get; set; }

    public int Quantity { get; set; }

    public decimal SellingPrice { get; set; }

    public int InvoiceId { get; set; }

    public int ProductId { get; set; }

    public virtual Invoice Invoice { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    [NotMapped]
    public decimal TotalPrice => Quantity * SellingPrice;

 
}
