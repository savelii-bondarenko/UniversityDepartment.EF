using ElectronicDepartment.DAL.Context;
using ElectronicDepartment.DAL.Entities;
using ElectronicDepartment.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ElectronicDepartment.DAL.Repositories;

public class StudentRepository(ElectronicDepartmentContext context) : Repository<Student>(context), IStudentRepository
{
    public virtual async Task<Student?> GetByEmailAsync(string email)
    {
        return await DbSet.FirstOrDefaultAsync(u => u.Email == email);
    }

    public virtual async Task<bool> EmailExistsAsync(string email)
    {
        return await DbSet.AnyAsync(u => u.Email == email);
    }

    public override async Task<List<Student>> GetAllAsync()
    {
        return await DbSet
            .Include(s => s.Group)
            .ThenInclude(g => g.Department)
            .ToListAsync();
    }

    public override async Task<Student?> GetByIdAsync(int id)
    {
        return await DbSet
            .Include(s => s.Group)
            .ThenInclude(g => g.Department)
            .Include(s => s.StudentSubjects)
            .ThenInclude(ss => ss.Subject)
            .ThenInclude(s => s.Teacher)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<Student>> GetByGroupIdAsync(int groupId)
    {
        return await DbSet
            .Include(s => s.Group)
            .Where(s => s.GroupId == groupId)
            .ToListAsync();
    }

    public async Task<List<Student>> GetStudentsWithGradesAsync()
    {
        return await DbSet
            .Include(s => s.Group)
            .Include(s => s.StudentSubjects.Where(ss => ss.Grade.HasValue))
            .ThenInclude(ss => ss.Subject)
            .ThenInclude(s => s.Teacher)
            .ToListAsync();
    }
}