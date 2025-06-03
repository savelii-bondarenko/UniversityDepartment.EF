using ElectronicDepartment.DAL.Entities;

namespace ElectronicDepartment.DAL.Repositories.Interfaces;

public interface IManagerRepository : IRepository<Manager>
{
    Task<Manager?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email);
}
