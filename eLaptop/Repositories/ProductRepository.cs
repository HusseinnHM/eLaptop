using System.Collections.Generic;
using System.Linq;
using eLaptop.Data;
using eLaptop.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eLaptop.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        IList<SelectListItem> GetAllDropdownList(string dropdownList);
    }

    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public IList<SelectListItem> GetAllDropdownList(string dropdownList)
        {
            return dropdownList switch
            {
                WebConstance.CompanyNames => _db.Companies.Select(i => new SelectListItem
                    {
                        Text = i.Name, Value = i.Id.ToString()
                    })
                    .ToList(),
                WebConstance.ProductTypeNames => _db.ProductTypes.Select(i => new SelectListItem
                    {
                        Text = i.Name, Value = i.Id.ToString()
                    })
                    .ToList(),
                _ => null
            };
        }


    }
}