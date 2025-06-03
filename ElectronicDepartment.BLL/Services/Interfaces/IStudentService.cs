using ElectronicDepartment.BLL.DTOs;
using ElectronicDepartment.DAL.Entities;

namespace ElectronicDepartment.BLL.Services.Interfaces;

public interface IStudentService
{
    Task<List<StudentDto>> GetAllAsync();
    //Task<List<StudentDto>> GetFilteredAsync(StudentFilterDto filter);
    Task<StudentDto?> GetByIdAsync(int id);
    Task<StudentDto> AddAsync(CreateStudentDto createDto);
    Task UpdateAsync(int id, UpdateStudentDto updateDto);
    Task DeleteAsync(int id);
    Task<List<GradeDto>> GetGradesByStudentIdAsync(int studentId);
    Task<List<SubjectDto>> GetSubjectsByStudentIdAsync(int studentId);
}
