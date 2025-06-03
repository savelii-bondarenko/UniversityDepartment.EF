using ElectronicDepartment.DAL.Entities;

namespace ElectronicDepartment.DAL.Repositories.Interfaces;

public interface IGroupRepository : IRepository<Group>
{
    Task<List<Group>> GetAllAsync();
    Task<Group?> GetByIdAsync(int id);
    Task<List<Group>> GetByDepartmentIdAsync(int departmentId);
    Task<bool> NameExistsInDepartmentAsync(string name, int departmentId, int? excludeId = null);
}
