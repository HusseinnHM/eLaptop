using System.Collections.Generic;

namespace eLaptop.ViewModels.Home
{
    public class HomeVM
    {
        public IList<HomeProductVM> Products { get; set; }
        public IList<CompanyVM> Companies { get; set; }
        public IList<ProductTypeVM> ProductTypes { get; set; }
    }
}