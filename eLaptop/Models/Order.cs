using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eLaptop.Models
{
    public class Order
    {
        [Key] 
        public int OrderId { get; set; }
        public Double TotalPrice { get; set; }
        public DateTime ShoppingDate { get; set; }

        [Required(ErrorMessage = "Please enter a city")]
        public string City { get; set; }

        [Required(ErrorMessage = "Please enter a Area")]
        public string Area { get; set; }

        [Required(ErrorMessage = "Please enter a Street Address")]
        public string StreetAddress { get; set; }
        
        public virtual IList<OrderItem> OrderItems { get; set; }

        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}