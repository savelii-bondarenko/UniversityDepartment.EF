using ElectronicDepartment.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace ElectronicDepartment.DAL.Repositories.Interfaces;

public interface ITeacherRepository : IRepository<Teacher>
{
    Task<Teacher?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email);
    Task<List<Teacher>> GetAllAsync();
    Task<Teacher?> GetByIdAsync(int id);
    Task<List<Teacher>> GetTeachersByDepartmentIdAsync(int departmentId);
}
