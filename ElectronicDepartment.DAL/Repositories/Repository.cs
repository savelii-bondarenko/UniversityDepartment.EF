using ElectronicDepartment.DAL.Context;
using ElectronicDepartment.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ElectronicDepartment.DAL.Repositories;

public class Repository<T>(ElectronicDepartmentContext context) : IRepository<T>
    where T : class
{
    protected readonly DbSet<T> DbSet = context.Set<T>();
    protected readonly ElectronicDepartmentContext Context = context;

    public virtual IQueryable<T> GetAllQueryable()
    {
        return DbSet.AsQueryable();
    }

    public virtual async Task<List<T>> GetAllAsync()
    {
        return await DbSet.ToListAsync();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await DbSet.FindAsync(id);
    }

    public virtual async Task AddAsync(T entity)
    {
        await DbSet.AddAsync(entity);
    }

    public virtual void Update(T entity)
    {
        DbSet.Update(entity);
    }

    public virtual void Remove(T entity)
    {
        DbSet.Remove(entity);
    }
}