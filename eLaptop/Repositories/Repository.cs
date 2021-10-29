using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using eLaptop.Data;
using eLaptop.Models;
using Microsoft.EntityFrameworkCore;

namespace eLaptop.Repositories
{
    public interface IRepository<T> where T : class
    {
       Task<IList<T>> GetAllAsync(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = null,
            bool isTracking = true);

       Task<T>  GetOneAsync(
            Expression<Func<T, bool>> filter = null,
            string includeProperties = null,
            bool isTracking = true
        );

       Task<T> FindAsync(int id);

      Task   AddAsync(T entity);

      void   Remove(T entity);

      void    Update(T entity);

      void    RemoveRange(IEnumerable<T> entity);

      Task   SaveAsync();
    }

    public class Repository<T> :IRepository<T> where T:class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;
        
        public Repository(ApplicationDbContext db)
        {
            _db = db;
            dbSet = _db.Set<T>();
        }
        public async Task<IList<T>> GetAllAsync(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = null,
            bool isTracking = true)
        {
            
            IQueryable<T> query = dbSet;
            
            if (filter != null)
                query = query.Where(filter);
            
            if (includeProperties != null)
                query = includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Aggregate(query, (current, includeProp) => current.Include(includeProp));

            if (!isTracking)
                query = query.AsNoTracking();
            
            if (orderBy != null)
                return await orderBy(query).ToListAsync();
            
            return await query.ToListAsync();
        }
        
        public async Task<T> GetOneAsync(Expression<Func<T, bool>> filter = null, string includeProperties = null, bool isTracking = true)
        {
            IQueryable<T> query = dbSet;
            
            if (filter != null)
                query = query.Where(filter);
            
            if (includeProperties != null)
                query = includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Aggregate(query, (current, includeProp) => current.Include(includeProp));

            if (!isTracking)
                return await query.AsNoTracking().FirstOrDefaultAsync();

            return await query.FirstOrDefaultAsync();
        }

        public async Task<T> FindAsync(int id)
            =>  await dbSet.FindAsync(id);

        public async Task AddAsync(T entity)
            => await dbSet.AddAsync(entity);

        public  void Remove(T entity)
            =>  dbSet.Remove(entity);

        public  void Update(T entity)
            =>  dbSet.Update(entity);

        public  void RemoveRange(IEnumerable<T> entities)
            =>  dbSet.RemoveRange(entities);

        public async Task SaveAsync()
            => await _db.SaveChangesAsync();
        public  void Save()
            => _db.SaveChanges();
    }
}