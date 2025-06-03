using ElectronicDepartment.DAL.Context;
using ElectronicDepartment.DAL.Entities;
using ElectronicDepartment.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ElectronicDepartment.DAL.Repositories;

public class StudentSubjectRepository(ElectronicDepartmentContext context) : Repository<StudentSubject>(context), IStudentSubjectRepository
{
    public override IQueryable<StudentSubject> GetAllQueryable()
    {
        return DbSet
            .Include(ss => ss.Student)
            .ThenInclude(s => s.Group)
            .Include(ss => ss.Subject)
            .ThenInclude(s => s.Teacher)
            .Include(ss => ss.Subject)
            .ThenInclude(s => s.Department);
    }

    public override async Task<List<StudentSubject>> GetAllAsync()
    {
        return await GetAllQueryable().ToListAsync();
    }

    public async Task<List<StudentSubject>> GetByStudentIdAsync(int studentId)
    {
        return await GetAllQueryable()
            .Where(ss => ss.StudentId == studentId)
            .ToListAsync();
    }

    public async Task<List<StudentSubject>> GetByTeacherIdAsync(int teacherId)
    {
        return await GetAllQueryable()
            .Where(ss => ss.Subject.TeacherId == teacherId)
            .ToListAsync();
    }

    public async Task<StudentSubject?> GetByStudentAndSubjectAsync(int studentId, int subjectId)
    {
        return await GetAllQueryable()
            .FirstOrDefaultAsync(ss => ss.StudentId == studentId && ss.SubjectId == subjectId);
    }

    public async Task<List<StudentSubject>> GetGradesByStudentIdAsync(int studentId)
    {
        return await GetAllQueryable()
            .Where(ss => ss.StudentId == studentId && ss.Grade.HasValue)
            .ToListAsync();
    }
}
