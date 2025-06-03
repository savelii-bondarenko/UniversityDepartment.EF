using AutoMapper;
using ElectronicDepartment.BLL.DTOs;
using ElectronicDepartment.Common.Exceptions;
using ElectronicDepartment.DAL.Entities;
using ElectronicDepartment.DAL.UnitOfWork.Interfaces;
using Moq;
using AppUnauthorizedAccessException = ElectronicDepartment.Common.Exceptions.UnauthorizedAccessException;

namespace ElectronicDepartment.Tests.Tests;

public class GradeServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly BLL.Services.GradeService _gradeService;

    public GradeServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _gradeService = new BLL.Services.GradeService(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task AddGradeAsync_ValidDto_AddsGrade()
    {
        var createDto = new CreateGradeDto { StudentId = 1, SubjectId = 1, TeacherId = 1, Grade = 5 };
        var student = new Student { Id = 1 };
        var subject = new Subject { Id = 1, TeacherId = 1 };
        var studentSubject = new StudentSubject { StudentId = 1, SubjectId = 1, Grade = 5 };
        var gradeDto = new GradeDto { Id = "1_1", StudentId = 1, SubjectId = 1, Grade = 5 };

        _unitOfWorkMock.Setup(u => u.Students.GetByIdAsync(1)).ReturnsAsync(student);
        _unitOfWorkMock.Setup(u => u.Subjects.GetByIdAsync(1)).ReturnsAsync(subject);
        _unitOfWorkMock.Setup(u => u.StudentSubjects.GetByStudentAndSubjectAsync(1, 1)).ReturnsAsync((StudentSubject)null);
        _unitOfWorkMock.Setup(u => u.StudentSubjects.AddAsync(It.IsAny<StudentSubject>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveAsync()).ReturnsAsync(1); 
        _unitOfWorkMock.Setup(u => u.StudentSubjects.GetByStudentAndSubjectAsync(1, 1)).ReturnsAsync(studentSubject);
        _mapperMock.Setup(m => m.Map<GradeDto>(studentSubject)).Returns(gradeDto);

        var result = await _gradeService.AddGradeAsync(createDto);

        Assert.NotNull(result);
        Assert.Equal(gradeDto, result);
    }

    [Fact]
    public async Task AddGradeAsync_NonExistingStudent_ThrowsEntityNotFoundException()
    {
        var createDto = new CreateGradeDto { StudentId = 1, SubjectId = 1, TeacherId = 1, Grade = 5 };
        _unitOfWorkMock.Setup(u => u.Students.GetByIdAsync(1)).ReturnsAsync((Student)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _gradeService.AddGradeAsync(createDto));
    }

    [Fact]
    public async Task AddGradeAsync_InvalidTeacher_ThrowsUnauthorizedAccessException()
    {
        var createDto = new CreateGradeDto { StudentId = 1, SubjectId = 1, TeacherId = 2, Grade = 5 };
        var student = new Student { Id = 1 };
        var subject = new Subject { Id = 1, TeacherId = 1 };

        _unitOfWorkMock.Setup(u => u.Students.GetByIdAsync(1)).ReturnsAsync(student);
        _unitOfWorkMock.Setup(u => u.Subjects.GetByIdAsync(1)).ReturnsAsync(subject);

        await Assert.ThrowsAsync<AppUnauthorizedAccessException>(() => _gradeService.AddGradeAsync(createDto));
    }

    [Fact]
    public async Task AddGradeAsync_InvalidGrade_ThrowsValidationException()
    {
        var createDto = new CreateGradeDto { StudentId = 1, SubjectId = 1, TeacherId = 1, Grade = 6 };
        var student = new Student { Id = 1 };
        var subject = new Subject { Id = 1, TeacherId = 1 };

        _unitOfWorkMock.Setup(u => u.Students.GetByIdAsync(1)).ReturnsAsync(student);
        _unitOfWorkMock.Setup(u => u.Subjects.GetByIdAsync(1)).ReturnsAsync(subject);

        await Assert.ThrowsAsync<ValidationException>(() => _gradeService.AddGradeAsync(createDto));
    }

    [Fact]
    public async Task DeleteGradeAsync_ValidIds_DeletesGrade()
    {
        var studentSubject = new StudentSubject
        {
            StudentId = 1,
            SubjectId = 1,
            Grade = 5,
            Subject = new Subject { TeacherId = 1 }
        };

        _unitOfWorkMock.Setup(u => u.StudentSubjects.GetByStudentAndSubjectAsync(1, 1)).ReturnsAsync(studentSubject);
        _unitOfWorkMock.Setup(u => u.StudentSubjects.Update(studentSubject)).Verifiable();
        _unitOfWorkMock.Setup(u => u.SaveAsync()).ReturnsAsync(1);

        await _gradeService.DeleteGradeAsync(1, 1, 1);

        _unitOfWorkMock.Verify(u => u.StudentSubjects.Update(It.Is<StudentSubject>(ss => ss.Grade == null)), Times.Once());
    }

    [Fact]
    public async Task DeleteGradeAsync_InvalidTeacher_ThrowsUnauthorizedAccessException()
    {
        var studentSubject = new StudentSubject
        {
            StudentId = 1,
            SubjectId = 1,
            Grade = 5,
            Subject = new Subject { TeacherId = 1 }
        };

        _unitOfWorkMock.Setup(u => u.StudentSubjects.GetByStudentAndSubjectAsync(1, 1)).ReturnsAsync(studentSubject);

        await Assert.ThrowsAsync<AppUnauthorizedAccessException>(() => _gradeService.DeleteGradeAsync(1, 1, 2));
    }
}