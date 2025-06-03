using ElectronicDepartment.DAL.Context;
using ElectronicDepartment.DAL.Entities;
using ElectronicDepartment.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ElectronicDepartment.DAL.Repositories;

public class TeacherRepository(ElectronicDepartmentContext context) : Repository<Teacher>(context), ITeacherRepository
{
    public virtual async Task<Teacher?> GetByEmailAsync(string email)
    {
        return await DbSet.FirstOrDefaultAsync(u => u.Email == email);
    }

    public virtual async Task<bool> EmailExistsAsync(string email)
    {
        return await DbSet.AnyAsync(u => u.Email == email);
    }
    public override async Task<List<Teacher>> GetAllAsync()
    {
        return await DbSet
            .Include(t => t.Subjects)
            .ThenInclude(s => s.Group)
            .ToListAsync();
    }

    public override async Task<Teacher?> GetByIdAsync(int id)
    {
        return await DbSet
            .Include(t => t.Subjects)
            .ThenInclude(s => s.Group)
            .Include(t => t.Subjects)
            .ThenInclude(s => s.Department)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<List<Teacher>> GetTeachersByDepartmentIdAsync(int departmentId)
    {
        return await DbSet
            .Where(t => t.Subjects.Any(s => s.DepartmentId == departmentId))
            .Include(t => t.Subjects.Where(s => s.DepartmentId == departmentId))
            .ToListAsync();
    }
}