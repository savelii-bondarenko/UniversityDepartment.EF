using ElectronicDepartment.DAL.Entities;

namespace ElectronicDepartment.DAL.Repositories.Interfaces;

public interface IDepartmentRepository : IRepository<Department>
{
    Task<List<Department>> GetAllAsync();
    Task<Department?> GetByIdAsync(int id);
    Task<Department?> GetByNameAsync(string name);
}
