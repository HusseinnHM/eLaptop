using eLaptop.Data;
using eLaptop.Models;

namespace eLaptop.Repositories
{
    public interface IProductTypeRepository : IRepository<ProductType>
    {
        
    }

    public class ProductTypeRepository :Repository<ProductType>, IProductTypeRepository
    {
        public ProductTypeRepository(ApplicationDbContext db) :base(db)
        {
            
        }
    }
}