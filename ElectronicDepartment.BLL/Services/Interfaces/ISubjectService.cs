using ElectronicDepartment.BLL.DTOs;

namespace ElectronicDepartment.BLL.Services.Interfaces;

public interface ISubjectService
{
    Task<List<SubjectDto>> GetAllAsync();
    Task<SubjectDto?> GetByIdAsync(int id);
    Task<SubjectDto> AddAsync(CreateSubjectDto createDto);
    Task UpdateAsync(UpdateSubjectDto updateDto);
    Task DeleteAsync(int id);
}