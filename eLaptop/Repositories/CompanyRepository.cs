using eLaptop.Data;
using eLaptop.Models;

namespace eLaptop.Repositories
{
    public interface ICompanyRepository : IRepository<Company>
    {
        
    }

    public class CompanyRepository :Repository<Company>, ICompanyRepository
    {
        public CompanyRepository(ApplicationDbContext db) :base(db)
        {
            
        }
    }
}