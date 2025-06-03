using ElectronicDepartment.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace ElectronicDepartment.DAL.Repositories.Interfaces;

public interface ISubjectRepository : IRepository<Subject>
{
    Task<List<Subject>> GetAllAsync();
    Task<Subject?> GetByIdAsync(int id);
    Task<List<Subject>> GetByTeacherIdAsync(int teacherId);
    Task<List<Subject>> GetByDepartmentIdAsync(int departmentId);
    Task<List<Subject>> GetByGroupIdAsync(int groupId);
}
