using System.ComponentModel.DataAnnotations;

namespace eLaptop.ViewModels.Account
{
    public class UpdateInfoVM
    {
        public string Id { get; set; }
        
        [Required]
        [Display(Name = "Full Name")]

        public string FullName { get; set; }
        public string Username { get; set; }

        [Required]
        public string City { get; set; }
        
        [Required]
        public string Area { get; set; }
        
        [Required] 
        public string StreetAddress { get; set; }
    }
}