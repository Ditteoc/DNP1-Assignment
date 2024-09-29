namespace RepositoryContracts;

public interface IRepository<T> where T : class
{
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
    Task<T> GetSingleAsync(int id);
    IQueryable<T> GetMany();
}