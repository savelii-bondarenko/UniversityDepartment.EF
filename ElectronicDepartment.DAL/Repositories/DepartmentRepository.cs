using ElectronicDepartment.DAL.Context;
using ElectronicDepartment.DAL.Entities;
using ElectronicDepartment.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ElectronicDepartment.DAL.Repositories;

public class DepartmentRepository(ElectronicDepartmentContext context) : Repository<Department>(context), IDepartmentRepository
{
    public override async Task<List<Department>> GetAllAsync()
    {
        return await DbSet
            .Include(d => d.Groups)
            .Include(d => d.Subjects)
            .ToListAsync();
    }

    public override async Task<Department?> GetByIdAsync(int id)
    {
        return await DbSet
            .Include(d => d.Groups)
            .ThenInclude(g => g.Students)
            .Include(d => d.Subjects)
            .ThenInclude(s => s.Teacher)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<Department?> GetByNameAsync(string name)
    {
        return await DbSet
            .Include(d => d.Groups)
            .Include(d => d.Subjects)
            .FirstOrDefaultAsync(d => d.Name == name);
    }
}
