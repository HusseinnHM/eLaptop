using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using eLaptop.Models;

namespace eLaptop.ViewModels.Home
{
    public class HomeProductVM
    {
      
        public int ProductId { get; set; }
        
        [Required(ErrorMessage = "Please enter a name of laptop")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please enter a description")]

        public double Price { get; set; }
        [Required(ErrorMessage = "Please enter a image link")]

        [Display(Name = "Image")]

        public string ImageUrl { get; set; }
        [Required(ErrorMessage = "Please enter a quantity")]

        [ForeignKey("CompanyId")] 
        public virtual Company Company { get; set; }
        
        [ForeignKey("ProductTypeId")] 
        public virtual ProductType ProductType { get; set; }

    }
}