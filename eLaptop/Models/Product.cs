using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eLaptop.Models
{
    public class Product
    {
        
        
        [Key]
        public int ProductId { get; set; }
        
        [Required(ErrorMessage = "Please enter a name of laptop")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please enter a description")]

        public string Description { get; set; }
        
        [Range(1,double.MaxValue,ErrorMessage = "Price should be greater than 0")]
        public double Price { get; set; }
        [Required(ErrorMessage = "Please enter a image link")]


        public string ImageUrl { get; set; }
        [Required(ErrorMessage = "Please enter a quantity")]
        
        [Range(1,uint.MaxValue,ErrorMessage = "Quantity should be positive")]

        public int Quantity { get; set; }

     
        
        public int CompanyId { get; set; }
        [ForeignKey("CompanyId")] 
        public virtual Company Company { get; set; }
        
        public int ProductTypeId { get; set; }
        [ForeignKey("ProductTypeId")] 
        public virtual ProductType ProductType { get; set; }

        
        
    }
}