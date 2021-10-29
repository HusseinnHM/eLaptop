using System;
using  eLaptop.Models;
namespace eLaptop.ViewModels.Order
{
    public class OrderItemVM
    {
       

        public int ProductId { get; set; }
      

        public string Name { get; set; }
        
        public int Amount { get; set; }

        public double Price { get; set; }
        
    }

   
}