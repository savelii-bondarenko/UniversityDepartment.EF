using ElectronicDepartment.BLL.DTOs;

namespace ElectronicDepartment.BLL.Services.Interfaces;

public interface IGroupService
{
    Task<GroupDto> AddAsync(CreateGroupDto createDto);
    Task<GroupDto?> GetByIdAsync(int id);
    Task<List<GroupDto>> GetAllAsync();
    Task<List<GroupDto>> GetFilteredAsync(GroupFilterDto filter);
    Task UpdateAsync(int id, UpdateGroupDto updateDto);
    Task DeleteAsync(int id);
}
