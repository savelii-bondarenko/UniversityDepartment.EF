using ElectronicDepartment.BLL.DTOs;
using ElectronicDepartment.DAL.Entities;

namespace ElectronicDepartment.BLL.Services.Interfaces;

public interface ITeacherService
{
    Task<List<TeacherDto>> GetAllAsync();
    //Task<List<TeacherDto>> GetFilteredAsync(TeacherFilterDto filter);
    Task<TeacherDto?> GetByIdAsync(int id);
    Task<TeacherDto?> GetByEmailAsync(string email);
    Task<TeacherDto> AddAsync(CreateTeacherDto createDto);
    Task UpdateAsync(int id, UpdateTeacherDto updateDto);
    Task DeleteAsync(int id);
    Task<List<SubjectDto>> GetSubjectsByTeacherIdAsync(int teacherId);
    Task<List<GradeDto>> GetGradesByTeacherIdAsync(int teacherId);
    Task<GradeDto> AddGradeAsync(CreateGradeDto createGradeDto);
    Task<GradeDto?> GetGradeByIdAsync(string id);
}
