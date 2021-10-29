using System.ComponentModel.DataAnnotations;

namespace eLaptop.Models
{
    public class ProductType
    {
        [Key] 
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter a laptop type name")]
        public string Name { get; set; }
        
    }
}