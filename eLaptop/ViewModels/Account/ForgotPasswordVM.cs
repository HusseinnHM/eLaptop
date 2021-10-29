using System.ComponentModel.DataAnnotations;

namespace eLaptop.ViewModels.Account
{
    public class ForgetPasswordVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}