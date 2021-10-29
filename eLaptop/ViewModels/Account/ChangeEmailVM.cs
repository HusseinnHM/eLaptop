using System.ComponentModel.DataAnnotations;

namespace eLaptop.ViewModels.Account
{
    public class ChangeEmailVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } 
        
        [Required]
        [EmailAddress]
        public string NewEmail { get; set; }
    }
}