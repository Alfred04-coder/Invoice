using System.ComponentModel.DataAnnotations;

namespace InvoiceManagement.ViewModels
{
    public class CompanyViewModel
    {
        public int CompanyId { get; set; }

        [Required]
        public string CompanyName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string? Phone { get; set; }
        [Display(Name = "Company Logo")]
        public IFormFile? LogoFile { get; set; }

        public string? Address { get; set; }

        [Required]
        public string AdminFullName { get; set; }

        [Required]
        [EmailAddress]
        public string AdminEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}