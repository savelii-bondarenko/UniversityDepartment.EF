using ElectronicDepartment.DAL.Context;
using ElectronicDepartment.DAL.Entities;
using ElectronicDepartment.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ElectronicDepartment.DAL.Repositories;

public class SubjectRepository(ElectronicDepartmentContext context) : Repository<Subject>(context), ISubjectRepository
{
    public override async Task<List<Subject>> GetAllAsync()
    {
        return await DbSet
            .Include(s => s.Department)
            .Include(s => s.Teacher)
            .Include(s => s.Group)
            .ToListAsync();
    }

    public override async Task<Subject?> GetByIdAsync(int id)
    {
        return await DbSet
            .Include(s => s.Department)
            .Include(s => s.Teacher)
            .Include(s => s.Group)
            .Include(s => s.StudentSubjects)
            .ThenInclude(ss => ss.Student)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<Subject>> GetByTeacherIdAsync(int teacherId)
    {
        return await DbSet
            .Include(s => s.Department)
            .Include(s => s.Group)
            .ThenInclude(g => g.Students)
            .Where(s => s.TeacherId == teacherId)
            .ToListAsync();
    }

    public async Task<List<Subject>> GetByDepartmentIdAsync(int departmentId)
    {
        return await DbSet
            .Include(s => s.Teacher)
            .Include(s => s.Group)
            .Where(s => s.DepartmentId == departmentId)
            .ToListAsync();
    }

    public async Task<List<Subject>> GetByGroupIdAsync(int groupId)
    {
        return await DbSet
            .Include(s => s.Department)
            .Include(s => s.Teacher)
            .Where(s => s.GroupId == groupId)
            .ToListAsync();
    }
}
