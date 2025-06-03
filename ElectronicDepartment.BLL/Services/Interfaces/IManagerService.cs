using ElectronicDepartment.BLL.DTOs;
using ElectronicDepartment.DAL.Entities;

namespace ElectronicDepartment.BLL.Services.Interfaces;

public interface IManagerService
{
    Task<List<ManagerDto>> GetAllAsync();
    Task<ManagerDto?> GetByIdAsync(int id);
    Task<ManagerDto> AddAsync(CreateManagerDto createDto);
    Task UpdateAsync(int id, UpdateManagerDto updateDto);
    Task DeleteAsync(int id);
}