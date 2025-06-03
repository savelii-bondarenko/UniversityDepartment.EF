using ElectronicDepartment.BLL.DTOs;

namespace ElectronicDepartment.BLL.Services.Interfaces;

public interface IGradeService
{
    Task<List<GradeDto>> GetAllAsync();
    Task<List<GradeDto>> GetFilteredAsync(GradeFilterDto filter);
    Task<List<GradeDto>> GetGradesByStudentIdAsync(int studentId);
    Task<List<GradeDto>> GetGradesByTeacherIdAsync(int teacherId);
    Task<GradeDto> AddGradeAsync(CreateGradeDto createDto);
    Task DeleteGradeAsync(int studentId, int subjectId, int teacherId);
}