using System;
using System.Collections.Generic;

namespace InvoiceManagement.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int InvoiceId { get; set; }

    public Invoice Invoice { get; set; }
    public string? ReceiptNumber { get; set; }

    public decimal BalanceAfterPayment { get; set; }

    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }

    public string Method { get; set; } // Cash, MoMo, Bank
    public string Reference { get; set; } // optional
}
