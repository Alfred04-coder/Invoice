using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvoiceManagement.Models;

public partial class Invoice
{
    public int InvoiceId { get; set; }
 
    public DateTime InvoiceDate { get; set; }

    public string CustomerName { get; set; } = null!;
    public string CustomerPhone { get; set; }

    public decimal GrandTotal { get; set; }

    public decimal Subtotal { get; set; }

    public decimal TaxAmount { get; set; }

    public string? TaxType { get; set; }

    public decimal TaxValue { get; set; }

    public decimal AmountPaid { get; set; }
    public string Status { get; set; } = "Issued";

    public string PaymentStatus { get; set; } = "Unpaid";

    public decimal Balance { get; set; }
    public bool IsCredit { get; set; }
    public bool IsDeleted { get; set; }

    public int CompanyId { get; set; }

    public int CustomerId { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;
    public ICollection<StockTransaction> StockTransactions { get; set; }

    public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
