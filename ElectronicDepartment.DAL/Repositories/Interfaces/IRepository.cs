namespace ElectronicDepartment.DAL.Repositories.Interfaces;

public interface IRepository<T> where T : class
{
    IQueryable<T> GetAllQueryable();
    Task<List<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
}