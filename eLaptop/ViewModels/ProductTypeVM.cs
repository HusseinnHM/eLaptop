using System.ComponentModel.DataAnnotations;

namespace eLaptop.ViewModels
{
    public class ProductTypeVM 
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter a company name")]
        public string Name { get; set; }
        
    }
}