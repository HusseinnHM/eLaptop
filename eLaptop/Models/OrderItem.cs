using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eLaptop.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }
        
        public string Name { get; set; }
        
        public int Amount { get; set; }
        
        public double Price { get; set; }
        
  

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        public int OrderId { get; set; }
        [ForeignKey("OrderId")]        
        public virtual Order Order { get; set; }
    }
}