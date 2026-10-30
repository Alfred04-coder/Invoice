using System;
using System.Collections.Generic;

namespace InvoiceManagement.Models;

public partial class SalaryPayment
{
    public int SalaryPaymentId { get; set; }

    public DateOnly Year { get; set; }

    public DateTime Month { get; set; }

    public bool IsPaid { get; set; }

    public DateTime PaidDate { get; set; }

    public int CompanyId { get; set; }

    public int EmplyeeId { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual Employee Emplyee { get; set; } = null!;
}
