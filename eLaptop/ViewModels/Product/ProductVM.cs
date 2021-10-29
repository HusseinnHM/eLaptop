using System.ComponentModel.DataAnnotations;

namespace eLaptop.ViewModels.Product
{
    public class ProductVM
    {
        public int ProductId { get; set; }
        
        [Required(ErrorMessage = "Please enter a name of laptop")]
        public string Name { get; set; }
   
        
        [Range(1,double.MaxValue,ErrorMessage = "Price should be greater than 0")]
        public double Price { get; set; }

   
        [Required(ErrorMessage = "Please enter a quantity")]
        
        [Range(1,uint.MaxValue,ErrorMessage = "Quantity should be positive")]

        public uint Quantity { get; set; }
        
        
    }
}