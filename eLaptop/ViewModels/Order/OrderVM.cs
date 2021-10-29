using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace eLaptop.ViewModels.Order
{
    public class OrderVM
    {

        public int OrderId { get; set; }
        public Double TotalPrice { get; set; }
        public DateTime ShoppingDate { get; set; }

        [Required(ErrorMessage = "Please enter a city")]
        public string City { get; set; }

        [Required(ErrorMessage = "Please enter a Area")]
        public string Area { get; set; }

        [Required(ErrorMessage = "Please enter a Street Address")]
        public string StreetAddress { get; set; }

        public IList<OrderItemVM> OrderItems { get; set; }
    }
}