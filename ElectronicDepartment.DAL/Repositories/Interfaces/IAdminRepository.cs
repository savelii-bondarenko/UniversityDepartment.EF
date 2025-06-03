using ElectronicDepartment.DAL.Entities;

namespace ElectronicDepartment.DAL.Repositories.Interfaces;

public interface IAdminRepository : IRepository<Admin>
{
    Task<Admin?> GetByEmailAsync(string email);

    Task<bool> EmailExistsAsync(string email);
}
