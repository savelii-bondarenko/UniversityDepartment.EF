using ElectronicDepartment.BLL.DTOs;
using ElectronicDepartment.DAL.Entities;

namespace ElectronicDepartment.BLL.Services.Interfaces;

public interface IAdminService
{
    Task<List<AdminDto>> GetAllAsync();
    Task<AdminDto?> GetByIdAsync(int id);
    Task<AdminDto> AddAsync(CreateAdminDto createDto);
    Task UpdateAsync(int id, UpdateAdminDto updateDto);
    Task DeleteAsync(int id);
}