using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace eLaptop.ViewModels.Account
{
    public class RegisterVM
    {
        [Required]
        public string FullName { get; set; }
        [Required]

        public string Username { get; set; }
        [Required]
        [EmailAddress]
        [Remote(controller:"Account",action:"IsEmailInUse")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password",
            ErrorMessage = "Password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        
        [Required]
        public string City { get; set; }
        
        [Required]
        public string Area { get; set; }
        
        [Required]
        public string StreetAddress { get; set; }

    }
}