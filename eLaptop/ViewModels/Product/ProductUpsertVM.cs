using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eLaptop.ViewModels.Product
{
    public class ProductUpsertVM : ProductVM
    {
        
        public string Description { get; set; }
        
        
        [Required]
        [Display(Name = "Image")]
        public string ImageUrl { get; set; }
        

        [Display(Name = "Company")]
        public int CompanyId { get; set; }
        
        [Display(Name = "Laptop type")]
        public int ProductTypeId { get; set; }
        
        public IList<SelectListItem> CompaniesList { get; set; }
        public IList<SelectListItem> ProductTypesList { get; set; }
    

    }
}