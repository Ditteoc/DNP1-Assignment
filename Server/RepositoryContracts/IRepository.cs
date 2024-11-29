using System.Linq.Expressions;

namespace RepositoryContracts;

public interface IRepository<T> where T : class
{
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
    Task<T> GetSingleAsync(int id);
    Task<IEnumerable<T>> GetManyAsync();
    Task<T?> GetSingleAsync(Expression<Func<T, bool>> predicate); // New overload
}