using ElectronicDepartment.DAL.Context;
using ElectronicDepartment.DAL.Entities;
using ElectronicDepartment.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ElectronicDepartment.DAL.Repositories;

public class ManagerRepository(ElectronicDepartmentContext context) : Repository<Manager>(context), IManagerRepository
{
    public virtual async Task<Manager?> GetByEmailAsync(string email)
    {
        return await DbSet.FirstOrDefaultAsync(u => u.Email == email);
    }

    public virtual async Task<bool> EmailExistsAsync(string email)
    {
        return await DbSet.AnyAsync(u => u.Email == email);
    }
}