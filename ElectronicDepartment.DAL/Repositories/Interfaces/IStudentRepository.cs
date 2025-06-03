using ElectronicDepartment.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace ElectronicDepartment.DAL.Repositories.Interfaces;

public interface IStudentRepository : IRepository<Student>
{
    Task<Student?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email);
    Task<List<Student>> GetAllAsync();
    Task<Student?> GetByIdAsync(int id);
    Task<List<Student>> GetByGroupIdAsync(int groupId);
    Task<List<Student>> GetStudentsWithGradesAsync();
}
