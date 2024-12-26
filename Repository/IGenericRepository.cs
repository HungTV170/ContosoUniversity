using System.Linq.Expressions;

namespace ContosoUniversity.Repository{
    public interface IGenericRepository<T> where T : class
    {
        Task<IList<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            List<string>? includeProperties = null
        );
        Task<T?> GetTAsync(
            Expression<Func<T, bool>>? filter = null,
            List<string>? includeProperties = null
        );
        Task AddAsync(T entity);
        void Update(T entity);
        Task DeleteAsync(int id);

        void DeleteEntity(T entity);
        void UpdateEntity(T entity);

        void UpdateRowVersion(T entity,string TimestampProperty,byte[] rowVersion);
    }

}