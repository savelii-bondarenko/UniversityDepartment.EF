using ElectronicDepartment.BLL.DTOs;
using ElectronicDepartment.BLL.Services.Interfaces;
using ElectronicDepartment.Common.Exceptions;
using ElectronicDepartment.DAL.Entities;
using ElectronicDepartment.DAL.UnitOfWork.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ElectronicDepartment.BLL.Services;

public class StudentService(IUnitOfWork unitOfWork, IMapper mapper) : IStudentService
{
    public async Task<List<StudentDto>> GetAllAsync()
    {
        var students = await unitOfWork.Students.GetAllAsync();
        return mapper.Map<List<StudentDto>>(students);
    }

    public async Task<List<StudentDto>> GetFilteredAsync(StudentFilterDto filter)
    {
        var query = unitOfWork.Students.GetAllQueryable();

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            query = query.Where(s => s.FirstName.Contains(filter.SearchTerm) ||
                                    s.LastName.Contains(filter.SearchTerm) ||
                                    s.Email.Contains(filter.SearchTerm));
        }

        if (filter.GroupId.HasValue)
            query = query.Where(s => s.GroupId == filter.GroupId.Value);

        if (filter.DepartmentId.HasValue)
            query = query.Where(s => s.Group.DepartmentId == filter.DepartmentId.Value);

        query = filter.SortBy switch
        {
            "FirstName" => filter.SortDescending ? query.OrderByDescending(s => s.FirstName) : query.OrderBy(s => s.FirstName),
            "LastName" => filter.SortDescending ? query.OrderByDescending(s => s.LastName) : query.OrderBy(s => s.LastName),
            _ => query.OrderBy(s => s.LastName)
        };

        var students = await query.ToListAsync();
        return mapper.Map<List<StudentDto>>(students);
    }

    public async Task<StudentDto?> GetByIdAsync(int id)
    {
        var student = await unitOfWork.Students.GetByIdAsync(id);
        return student == null ? null : mapper.Map<StudentDto>(student);
    }

    public async Task<StudentDto> AddAsync(CreateStudentDto createDto)
    {
        if (string.IsNullOrWhiteSpace(createDto.Email))
            throw new ValidationException("Email не може бути порожнім");

        if (await unitOfWork.Students.EmailExistsAsync(createDto.Email))
            throw new ValidationException("Студент з таким email вже існує");

        var group = await unitOfWork.Groups.GetByIdAsync(createDto.GroupId);
        if (group == null)
            throw new EntityNotFoundException("Group", createDto.GroupId);

        var student = mapper.Map<Student>(createDto);
        student.PasswordHash = HashPassword(createDto.Password);
        student.Role = Common.Enums.UserRole.Student;

        await unitOfWork.Students.AddAsync(student);
        await unitOfWork.SaveAsync();

        return mapper.Map<StudentDto>(student);
    }

    public async Task UpdateAsync(int id, UpdateStudentDto updateDto)
    {
        var student = await unitOfWork.Students.GetByIdAsync(id);
        if (student == null)
            throw new EntityNotFoundException("Student", id);

        if (updateDto.GroupId >= 0)
        {
            var group = await unitOfWork.Groups.GetByIdAsync(updateDto.GroupId);
            if (group == null)
                throw new EntityNotFoundException("Group", updateDto.GroupId);
        }

        mapper.Map(updateDto, student);
        unitOfWork.Students.Update(student);
        await unitOfWork.SaveAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var student = await unitOfWork.Students.GetByIdAsync(id);
        if (student == null)
            return;

        unitOfWork.Students.Remove(student);
        await unitOfWork.SaveAsync();
    }

    public async Task<List<GradeDto>> GetGradesByStudentIdAsync(int studentId)
    {
        var studentSubjects = await unitOfWork.StudentSubjects.GetByStudentIdAsync(studentId);
        return mapper.Map<List<GradeDto>>(studentSubjects.Where(ss => ss.Grade.HasValue));
    }

    public async Task<List<SubjectDto>> GetSubjectsByStudentIdAsync(int studentId)
    {
        var studentSubjects = await unitOfWork.StudentSubjects.GetByStudentIdAsync(studentId);
        var subjects = studentSubjects.Select(ss => ss.Subject).ToList();
        return mapper.Map<List<SubjectDto>>(subjects);
    }

    private string HashPassword(string password)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}
