using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace InvoiceManagement.Models;

public partial class ApplicationUser : IdentityUser
{
   
 

    public int? CompanyId { get; set; }

    public virtual Company? Company { get; set; }
}
