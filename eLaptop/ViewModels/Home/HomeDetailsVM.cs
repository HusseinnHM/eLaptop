using System.ComponentModel.DataAnnotations;

namespace eLaptop.ViewModels.Home
{
    public class HomeDetailsVM : HomeProductVM
    {
        [Required(ErrorMessage = "Please enter a description")]
        public string Description { get; set; }
        
    
    }
}