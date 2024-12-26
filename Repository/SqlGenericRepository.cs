using System.Linq.Expressions;
using System.Net.WebSockets;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Repository{
    public class SqlGenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly SchoolContext _context;

        private readonly DbSet<T> _dbSet;

        public SqlGenericRepository(SchoolContext context)
        {
    
            _context = context;
            _dbSet = context.Set<T>();
        }
        
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);

            if (entity == null){
                throw new Exception("Entity not found");
            }

             _dbSet.Remove(entity);
        }

        public async Task<IList<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, List<string>? includeProperties = null)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            if( orderBy != null)
            {
                query =  orderBy(query);
            }

            return await query.AsNoTracking().ToListAsync();


        }

        public async Task<T?> GetTAsync(Expression<Func<T, bool>>? filter = null, List<string>? includeProperties = null)
        {
            IQueryable<T> query = _dbSet;

            if(includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            if(filter == null)
            {
                throw new Exception("Filter is required");
            }

            return await query.AsNoTracking().FirstOrDefaultAsync(filter);

        }

        public void Update(T entity)
        {
            _dbSet.Attach(entity);
            _dbSet.Entry(entity).State = EntityState.Modified;            
        }

        public void UpdateEntity(T entity)
        {
            _dbSet.Update(entity);
        }

        public void DeleteEntity(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void UpdateRowVersion(T entity,string TimestampProperty, byte[] rowVersion)
        {
            if(_context.Entry(entity).State == EntityState.Detached){
                _dbSet.Attach(entity);
            }
            _context.Entry(entity).Property(TimestampProperty).OriginalValue = rowVersion;
        }

    }

}