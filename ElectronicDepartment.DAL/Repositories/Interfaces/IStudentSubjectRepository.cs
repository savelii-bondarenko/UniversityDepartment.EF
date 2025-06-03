using ElectronicDepartment.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace ElectronicDepartment.DAL.Repositories.Interfaces;

public interface IStudentSubjectRepository : IRepository<StudentSubject>
{
    IQueryable<StudentSubject> GetAllQueryable();
    Task<List<StudentSubject>> GetAllAsync();
    Task<List<StudentSubject>> GetByStudentIdAsync(int studentId);
    Task<List<StudentSubject>> GetByTeacherIdAsync(int teacherId);
    Task<StudentSubject?> GetByStudentAndSubjectAsync(int studentId, int subjectId);
    Task<List<StudentSubject>> GetGradesByStudentIdAsync(int studentId);
}
