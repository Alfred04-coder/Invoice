using System;
using System.Collections.Generic;

namespace InvoiceManagement.Models;

public partial class Customer
{
    public int CustomerId { get; set; }
    public int CompanyId { get; set; }

    public string CustomerName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Address { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
