using System.Collections.Generic;

namespace eLaptop.ViewModels
{
    public class ShoppingCartVM
    {
        public IList<ShoppingCartItemVM> ShoppingCartItems { get; set; }

        public double ShoppingCartTotal { get; set; }
    }
}