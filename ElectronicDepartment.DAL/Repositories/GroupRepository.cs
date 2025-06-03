using ElectronicDepartment.DAL.Context;
using ElectronicDepartment.DAL.Entities;
using ElectronicDepartment.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ElectronicDepartment.DAL.Repositories;

public class GroupRepository(ElectronicDepartmentContext context) : Repository<Group>(context), IGroupRepository
{
    public override async Task<List<Group>> GetAllAsync()
    {
        return await DbSet
            .Include(g => g.Department)
            .Include(g => g.Students)
            .ToListAsync();
    }

    public override async Task<Group?> GetByIdAsync(int id)
    {
        return await DbSet
            .Include(g => g.Department)
            .Include(g => g.Students)
            .Include(g => g.Subjects)
            .ThenInclude(s => s.Teacher)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<List<Group>> GetByDepartmentIdAsync(int departmentId)
    {
        return await DbSet
            .Include(g => g.Department)
            .Include(g => g.Students)
            .Where(g => g.DepartmentId == departmentId)
            .ToListAsync();
    }

    public async Task<bool> NameExistsInDepartmentAsync(string name, int departmentId, int? excludeId = null)
    {
        return await DbSet
            .AnyAsync(g => g.Name == name && g.DepartmentId == departmentId && (!excludeId.HasValue || g.Id != excludeId.Value));
    }
}
