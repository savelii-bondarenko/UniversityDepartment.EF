using System.Text;
using AutoMapper;
using ElectronicDepartment.BLL.DTOs;
using ElectronicDepartment.BLL.Services.Interfaces;
using ElectronicDepartment.Common.Exceptions;
using ElectronicDepartment.DAL.Entities;
using ElectronicDepartment.DAL.UnitOfWork.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace ElectronicDepartment.BLL.Services;

public class TeacherService(IUnitOfWork unitOfWork, IMapper mapper) : ITeacherService
{
    public async Task<List<TeacherDto>> GetAllAsync()
    {
        var teachers = await unitOfWork.Teachers.GetAllAsync();
        return mapper.Map<List<TeacherDto>>(teachers);
    }

    public async Task<List<TeacherDto>> GetFilteredAsync(TeacherFilterDto filter)
    {
        // Валідація вхідного фільтра
        if (filter == null)
        {
            throw new ValidationException("Фільтр не може бути порожнім.");
        }

        // Створюємо базовий запит
        var query = unitOfWork.Teachers.GetAllQueryable();

        // Пошук за SearchTerm (за ім'ям, прізвищем або email)
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var searchTerm = filter.SearchTerm.ToLower();
            query = query.Where(t => t.FirstName.ToLower().Contains(searchTerm) ||
                                    t.LastName.ToLower().Contains(searchTerm) ||
                                    t.Email.ToLower().Contains(searchTerm));
        }

        // Сортування
        if (!string.IsNullOrWhiteSpace(filter.SortBy))
        {
            query = filter.SortBy switch
            {
                "FirstName" => filter.SortDescending ? query.OrderByDescending(t => t.FirstName) : query.OrderBy(t => t.FirstName),
                "LastName" => filter.SortDescending ? query.OrderByDescending(t => t.LastName) : query.OrderBy(t => t.LastName),
                "Email" => filter.SortDescending ? query.OrderByDescending(t => t.Email) : query.OrderBy(t => t.Email),
                _ => query.OrderBy(t => t.LastName) // Сортування за замовчуванням
            };
        }
        else
        {
            // Сортування за замовчуванням, якщо SortBy не вказано
            query = query.OrderBy(t => t.LastName);
        }

        // Виконання запиту
        var teachers = await query.ToListAsync();

        // Мапінг результатів на DTO
        var teacherDtos = mapper.Map<List<TeacherDto>>(teachers);
        return teacherDtos;
    }

    public async Task<TeacherDto?> GetByIdAsync(int id)
    {
        var teacher = await unitOfWork.Teachers.GetByIdAsync(id);
        return teacher == null ? null : mapper.Map<TeacherDto>(teacher);
    }

    public async Task<TeacherDto?> GetByEmailAsync(string email)
    {
        var teacher = await unitOfWork.Teachers.GetByEmailAsync(email);
        return teacher == null ? null : mapper.Map<TeacherDto>(teacher);
    }

    public async Task<TeacherDto> AddAsync(CreateTeacherDto createDto)
    {
        if (string.IsNullOrWhiteSpace(createDto.Email))
            throw new ValidationException("Email не може бути порожнім");

        if (await unitOfWork.Teachers.EmailExistsAsync(createDto.Email))
            throw new ValidationException("Викладач з таким email вже існує");

        var teacher = mapper.Map<Teacher>(createDto);
        teacher.PasswordHash = HashPassword(createDto.Password);

        await unitOfWork.BeginTransactionAsync();
        try
        {
            await unitOfWork.Teachers.AddAsync(teacher);
            await unitOfWork.SaveAsync();
            await unitOfWork.CommitTransactionAsync();

            return mapper.Map<TeacherDto>(teacher);
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task UpdateAsync(int id, UpdateTeacherDto updateDto)
    {
        var teacher = await unitOfWork.Teachers.GetByIdAsync(id);
        if (teacher == null)
            throw new EntityNotFoundException("Teacher", id);


        await unitOfWork.BeginTransactionAsync();
        try
        {
            mapper.Map(updateDto, teacher);
            unitOfWork.Teachers.Update(teacher);
            await unitOfWork.SaveAsync();
            await unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task DeleteAsync(int id)
    {
        var teacher = await unitOfWork.Teachers.GetByIdAsync(id);
        if (teacher == null)
            return;

        await unitOfWork.BeginTransactionAsync();
        try
        {
            unitOfWork.Teachers.Remove(teacher);
            await unitOfWork.SaveAsync();
            await unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<List<SubjectDto>> GetSubjectsByTeacherIdAsync(int teacherId)
    {
        var subjects = await unitOfWork.Subjects.GetByTeacherIdAsync(teacherId);
        return mapper.Map<List<SubjectDto>>(subjects);
    }

    public async Task<List<GradeDto>> GetGradesByTeacherIdAsync(int teacherId)
    {
        var studentSubjects = await unitOfWork.StudentSubjects.GetByTeacherIdAsync(teacherId);
        return mapper.Map<List<GradeDto>>(studentSubjects.Where(ss => ss.Grade.HasValue));
    }

    public async Task<GradeDto> AddGradeAsync(CreateGradeDto createGradeDto)
    {
        await unitOfWork.BeginTransactionAsync();
        try
        {
            var student = await unitOfWork.Students.GetByIdAsync(createGradeDto.StudentId);
            if (student == null)
                throw new EntityNotFoundException("Student", createGradeDto.StudentId);

            var subject = await unitOfWork.Subjects.GetByIdAsync(createGradeDto.SubjectId);
            if (subject == null)
                throw new EntityNotFoundException("Subject", createGradeDto.SubjectId);

            if (subject.TeacherId != createGradeDto.TeacherId)
                throw new Common.Exceptions.UnauthorizedAccessException("Викладач не має права ставити оцінку для цього предмета.");

            var studentSubject = await unitOfWork.StudentSubjects
                .GetByStudentAndSubjectAsync(createGradeDto.StudentId, createGradeDto.SubjectId);
            if (studentSubject == null)
            {
                if (student.GroupId != subject.GroupId)
                    throw new ValidationException("Студент не зарахований на цей предмет.");

                studentSubject = new StudentSubject
                {
                    StudentId = createGradeDto.StudentId,
                    SubjectId = createGradeDto.SubjectId,
                    Grade = createGradeDto.Grade
                };
                await unitOfWork.StudentSubjects.AddAsync(studentSubject);
            }
            else
            {
                studentSubject.Grade = createGradeDto.Grade;
                unitOfWork.StudentSubjects.Update(studentSubject);
            }

            await unitOfWork.SaveAsync();
            await unitOfWork.CommitTransactionAsync();

            return mapper.Map<GradeDto>(studentSubject);
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<GradeDto?> GetGradeByIdAsync(string id)
    {
        // Розпарсуємо id у форматі StudentId_SubjectId
        var ids = id.Split('_');
        if (ids.Length != 2 || !int.TryParse(ids[0], out var studentId) || !int.TryParse(ids[1], out var subjectId))
            throw new ValidationException("Невалідний формат ID оцінки. Очікується формат 'StudentId_SubjectId'.");

        var studentSubject = await unitOfWork.StudentSubjects
            .GetByStudentAndSubjectAsync(studentId, subjectId);
        if (studentSubject == null)
            return null;

        var gradeDto = mapper.Map<GradeDto>(studentSubject);

        return gradeDto;
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}
