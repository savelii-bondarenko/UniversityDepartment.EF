using ElectronicDepartment.BLL.DTOs;
using ElectronicDepartment.BLL.Services.Interfaces;
using ElectronicDepartment.Common.Exceptions;
using ElectronicDepartment.DAL.Entities;
using ElectronicDepartment.DAL.UnitOfWork.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ElectronicDepartment.BLL.Services;

public class GradeService(IUnitOfWork unitOfWork, IMapper mapper) : IGradeService
{
    public async Task<List<GradeDto>> GetAllAsync()
    {
        var studentSubjects = await unitOfWork.StudentSubjects.GetAllAsync();
        return mapper.Map<List<GradeDto>>(studentSubjects.Where(ss => ss.Grade.HasValue));
    }

    public async Task<List<GradeDto>> GetFilteredAsync(GradeFilterDto filter)
    {
        var query = unitOfWork.StudentSubjects.GetAllQueryable()
            .Where(ss => ss.Grade.HasValue);

        if (filter.StudentId.HasValue)
            query = query.Where(ss => ss.StudentId == filter.StudentId.Value);

        if (filter.SubjectId.HasValue)
            query = query.Where(ss => ss.SubjectId == filter.SubjectId.Value);

        if (filter.TeacherId.HasValue)
            query = query.Where(ss => ss.Subject.TeacherId == filter.TeacherId.Value);

        if (filter.MinGrade.HasValue)
            query = query.Where(ss => ss.Grade >= filter.MinGrade.Value);

        if (filter.MaxGrade.HasValue)
            query = query.Where(ss => ss.Grade <= filter.MaxGrade.Value);

        var results = await query.ToListAsync();
        return mapper.Map<List<GradeDto>>(results);
    }

    public async Task<List<GradeDto>> GetGradesByStudentIdAsync(int studentId)
    {
        var studentSubjects = await unitOfWork.StudentSubjects.GetByStudentIdAsync(studentId);
        return mapper.Map<List<GradeDto>>(studentSubjects.Where(ss => ss.Grade.HasValue));
    }

    public async Task<List<GradeDto>> GetGradesByTeacherIdAsync(int teacherId)
    {
        var studentSubjects = await unitOfWork.StudentSubjects.GetByTeacherIdAsync(teacherId);
        return mapper.Map<List<GradeDto>>(studentSubjects.Where(ss => ss.Grade.HasValue));
    }

    public async Task<GradeDto> AddGradeAsync(CreateGradeDto createDto)
    {
        var student = await unitOfWork.Students.GetByIdAsync(createDto.StudentId);
        if (student == null)
            throw new EntityNotFoundException("Student", createDto.StudentId);

        var subject = await unitOfWork.Subjects.GetByIdAsync(createDto.SubjectId);
        if (subject == null)
            throw new EntityNotFoundException("Subject", createDto.SubjectId);

        if (subject.TeacherId != createDto.TeacherId)
            throw new Common.Exceptions.UnauthorizedAccessException("Викладач не має права ставити оцінку для цього предмета");

        if (createDto.Grade < 1 || createDto.Grade > 5)
            throw new ValidationException("Оцінка повинна бути від 1 до 5");

        var studentSubject = await unitOfWork.StudentSubjects
            .GetByStudentAndSubjectAsync(createDto.StudentId, createDto.SubjectId);

        if (studentSubject == null)
        {
            studentSubject = new StudentSubject
            {
                StudentId = createDto.StudentId,
                SubjectId = createDto.SubjectId,
                Grade = createDto.Grade
            };
            await unitOfWork.StudentSubjects.AddAsync(studentSubject);
        }
        else
        {
            studentSubject.Grade = createDto.Grade;
            unitOfWork.StudentSubjects.Update(studentSubject);
        }

        await unitOfWork.SaveAsync();

        studentSubject = await unitOfWork.StudentSubjects
            .GetByStudentAndSubjectAsync(createDto.StudentId, createDto.SubjectId);

        return mapper.Map<GradeDto>(studentSubject);
    }

    public async Task DeleteGradeAsync(int studentId, int subjectId, int teacherId)
    {
        var studentSubject = await unitOfWork.StudentSubjects
            .GetByStudentAndSubjectAsync(studentId, subjectId);

        if (studentSubject == null)
            return;

        if (studentSubject.Subject.TeacherId != teacherId)
            throw new Common.Exceptions.UnauthorizedAccessException("Викладач не має права видаляти цю оцінку");

        studentSubject.Grade = null;
        unitOfWork.StudentSubjects.Update(studentSubject);
        await unitOfWork.SaveAsync();
    }
}
