using ElectronicDepartment.BLL.DTOs;

namespace ElectronicDepartment.BLL.Services.Interfaces;

public interface IDepartmentService
{
    Task<List<DepartmentDto>> GetAllAsync();
    Task<DepartmentDto?> GetByIdAsync(int id);
    Task<DepartmentDto> AddAsync(CreateDepartmentDto createDto);
    Task UpdateAsync(int id, UpdateDepartmentDto updateDto);
    Task DeleteAsync(int id);
    Task<List<SubjectDto>> GetSubjectsByDepartmentIdAsync(int departmentId);
    Task<List<TeacherDto>> GetTeachersByDepartmentIdAsync(int departmentId);
    Task<List<GroupDto>> GetGroupsByDepartmentIdAsync(int departmentId);
}
