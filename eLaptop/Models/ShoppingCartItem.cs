using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eLaptop.Models
{
    public class ShoppingCartItem
    {

        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        
        public int Amount { get; set; }
        public string ShoppingCartId { get; set; }
        
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

    }
}