using AutoMapper;
using ElectronicDepartment.BLL.DTOs;
using ElectronicDepartment.Common.Exceptions;
using ElectronicDepartment.DAL.Entities;
using ElectronicDepartment.DAL.UnitOfWork.Interfaces;
using Moq;

namespace ElectronicDepartment.Tests.Tests.TeacherService;

public class TeacherServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly BLL.Services.TeacherService _teacherService;

    public TeacherServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _teacherService = new BLL.Services.TeacherService(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsListOfTeacherDtos()
    {
        var teachers = new List<Teacher> { new Teacher { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" } };
        var teacherDtos = new List<TeacherDto> { new TeacherDto { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" } };

        _unitOfWorkMock.Setup(u => u.Teachers.GetAllAsync()).ReturnsAsync(teachers);
        _mapperMock.Setup(m => m.Map<List<TeacherDto>>(teachers)).Returns(teacherDtos);

        var result = await _teacherService.GetAllAsync();

        Assert.NotNull(result);
        Assert.Equal(teacherDtos.Count, result.Count);
        Assert.Equal(teacherDtos[0].Id, result[0].Id);
        Assert.Equal(teacherDtos[0].FirstName, result[0].FirstName);
        Assert.Equal(teacherDtos[0].LastName, result[0].LastName);
        Assert.Equal(teacherDtos[0].Email, result[0].Email);
        _unitOfWorkMock.Verify(u => u.Teachers.GetAllAsync(), Times.Once());
        _mapperMock.Verify(m => m.Map<List<TeacherDto>>(teachers), Times.Once());
    }

    [Fact]
    public async Task GetFilteredAsync_NullFilter_ThrowsValidationException()
    {
        await Assert.ThrowsAsync<ValidationException>(() => _teacherService.GetFilteredAsync(null));
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsTeacherDto()
    {
        var teacher = new Teacher { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" };
        var teacherDto = new TeacherDto { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" };

        _unitOfWorkMock.Setup(u => u.Teachers.GetByIdAsync(1)).ReturnsAsync(teacher);
        _mapperMock.Setup(m => m.Map<TeacherDto>(teacher)).Returns(teacherDto);

        var result = await _teacherService.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(teacherDto.Id, result.Id);
        Assert.Equal(teacherDto.FirstName, result.FirstName);
        Assert.Equal(teacherDto.LastName, result.LastName);
        Assert.Equal(teacherDto.Email, result.Email);
        _unitOfWorkMock.Verify(u => u.Teachers.GetByIdAsync(1), Times.Once());
        _mapperMock.Verify(m => m.Map<TeacherDto>(teacher), Times.Once());
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        _unitOfWorkMock.Setup(u => u.Teachers.GetByIdAsync(1)).ReturnsAsync((Teacher)null);

        var result = await _teacherService.GetByIdAsync(1);

        Assert.Null(result);
        _unitOfWorkMock.Verify(u => u.Teachers.GetByIdAsync(1), Times.Once());
    }

    [Fact]
    public async Task GetByEmailAsync_ExistingEmail_ReturnsTeacherDto()
    {
        var teacher = new Teacher { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" };
        var teacherDto = new TeacherDto { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" };

        _unitOfWorkMock.Setup(u => u.Teachers.GetByEmailAsync("john.doe@example.com")).ReturnsAsync(teacher);
        _mapperMock.Setup(m => m.Map<TeacherDto>(teacher)).Returns(teacherDto);

        var result = await _teacherService.GetByEmailAsync("john.doe@example.com");

        Assert.NotNull(result);
        Assert.Equal(teacherDto.Id, result.Id);
        Assert.Equal(teacherDto.FirstName, result.FirstName);
        Assert.Equal(teacherDto.LastName, result.LastName);
        Assert.Equal(teacherDto.Email, result.Email);
        _unitOfWorkMock.Verify(u => u.Teachers.GetByEmailAsync("john.doe@example.com"), Times.Once());
        _mapperMock.Verify(m => m.Map<TeacherDto>(teacher), Times.Once());
    }

    [Fact]
    public async Task GetByEmailAsync_NonExistingEmail_ReturnsNull()
    {
        _unitOfWorkMock.Setup(u => u.Teachers.GetByEmailAsync("john.doe@example.com")).ReturnsAsync((Teacher)null);

        var result = await _teacherService.GetByEmailAsync("john.doe@example.com");

        Assert.Null(result);
        _unitOfWorkMock.Verify(u => u.Teachers.GetByEmailAsync("john.doe@example.com"), Times.Once());
    }

    [Fact]
    public async Task AddAsync_ValidDto_AddsTeacherAndReturnsDto()
    {
        var createDto = new CreateTeacherDto { FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", Password = "password" };
        var teacher = new Teacher { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", Role = Common.Enums.UserRole.Teacher };
        var teacherDto = new TeacherDto { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" };

        _unitOfWorkMock.Setup(u => u.Teachers.EmailExistsAsync(createDto.Email)).ReturnsAsync(false);
        _mapperMock.Setup(m => m.Map<Teacher>(createDto)).Returns(teacher);
        _mapperMock.Setup(m => m.Map<TeacherDto>(teacher)).Returns(teacherDto);
        _unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.Teachers.AddAsync(teacher)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveAsync()).ReturnsAsync(1);
        _unitOfWorkMock.Setup(u => u.CommitTransactionAsync()).Returns(Task.CompletedTask);

        var result = await _teacherService.AddAsync(createDto);

        Assert.NotNull(result);
        Assert.Equal(teacherDto.Id, result.Id);
        Assert.Equal(teacherDto.FirstName, result.FirstName);
        Assert.Equal(teacherDto.LastName, result.LastName);
        Assert.Equal(teacherDto.Email, result.Email);
        _unitOfWorkMock.Verify(u => u.Teachers.AddAsync(It.Is<Teacher>(t => t.Role == Common.Enums.UserRole.Teacher)), Times.Once());
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once());
        _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Once());
        _mapperMock.Verify(m => m.Map<TeacherDto>(teacher), Times.Once());
    }

    [Fact]
    public async Task AddAsync_EmptyEmail_ThrowsValidationException()
    {
        var createDto = new CreateTeacherDto { FirstName = "John", LastName = "Doe", Email = "", Password = "password" };

        await Assert.ThrowsAsync<ValidationException>(() => _teacherService.AddAsync(createDto));
    }

    [Fact]
    public async Task AddAsync_ExistingEmail_ThrowsValidationException()
    {
        var createDto = new CreateTeacherDto { FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", Password = "password" };
        _unitOfWorkMock.Setup(u => u.Teachers.EmailExistsAsync(createDto.Email)).ReturnsAsync(true);

        await Assert.ThrowsAsync<ValidationException>(() => _teacherService.AddAsync(createDto));
    }

    [Fact]
    public async Task UpdateAsync_ValidDto_UpdatesTeacher()
    {
        var updateDto = new UpdateTeacherDto { FirstName = "Jane", LastName = "Doe", Email = "jane.doe@example.com" };
        var teacher = new Teacher { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" };

        _unitOfWorkMock.Setup(u => u.Teachers.GetByIdAsync(1)).ReturnsAsync(teacher);
        _unitOfWorkMock.Setup(u => u.Teachers.Update(teacher)).Verifiable();
        _unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveAsync()).ReturnsAsync(1);
        _unitOfWorkMock.Setup(u => u.CommitTransactionAsync()).Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map(updateDto, teacher)).Verifiable();

        await _teacherService.UpdateAsync(1, updateDto);

        _unitOfWorkMock.Verify(u => u.Teachers.Update(teacher), Times.Once());
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once());
        _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Once());
        _mapperMock.Verify(m => m.Map(updateDto, teacher), Times.Once());
    }

    [Fact]
    public async Task UpdateAsync_NonExistingTeacher_ThrowsEntityNotFoundException()
    {
        var updateDto = new UpdateTeacherDto { FirstName = "Jane", LastName = "Doe", Email = "jane.doe@example.com" };
        _unitOfWorkMock.Setup(u => u.Teachers.GetByIdAsync(1)).ReturnsAsync((Teacher)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _teacherService.UpdateAsync(1, updateDto));
    }

    [Fact]
    public async Task DeleteAsync_ExistingId_DeletesTeacher()
    {
        var teacher = new Teacher { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" };
        _unitOfWorkMock.Setup(u => u.Teachers.GetByIdAsync(1)).ReturnsAsync(teacher);
        _unitOfWorkMock.Setup(u => u.Teachers.Remove(teacher)).Verifiable();
        _unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveAsync()).ReturnsAsync(1);
        _unitOfWorkMock.Setup(u => u.CommitTransactionAsync()).Returns(Task.CompletedTask);

        await _teacherService.DeleteAsync(1);

        _unitOfWorkMock.Verify(u => u.Teachers.Remove(teacher), Times.Once());
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once());
        _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Once());
    }

    [Fact]
    public async Task DeleteAsync_NonExistingId_DoesNothing()
    {
        _unitOfWorkMock.Setup(u => u.Teachers.GetByIdAsync(1)).ReturnsAsync((Teacher)null);

        await _teacherService.DeleteAsync(1);

        _unitOfWorkMock.Verify(u => u.Teachers.Remove(It.IsAny<Teacher>()), Times.Never());
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Never());
        _unitOfWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Never());
    }

    [Fact]
    public async Task GetGradesByTeacherIdAsync_ReturnsGradeDtos()
    {
        var studentSubjects = new List<StudentSubject> { new StudentSubject { StudentId = 1, SubjectId = 1, Grade = 5 } };
        var gradeDtos = new List<GradeDto> { new GradeDto { Id = "1_1", StudentId = 1, SubjectId = 1, Grade = 5 } };

        _unitOfWorkMock.Setup(u => u.StudentSubjects.GetByTeacherIdAsync(1)).ReturnsAsync(studentSubjects);
        _mapperMock.Setup(m => m.Map<List<GradeDto>>(studentSubjects)).Returns(gradeDtos);

        var result = await _teacherService.GetGradesByTeacherIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(gradeDtos.Count, result.Count);
        Assert.Equal(gradeDtos[0].Id, result[0].Id);
        Assert.Equal(gradeDtos[0].StudentId, result[0].StudentId);
        Assert.Equal(gradeDtos[0].SubjectId, result[0].SubjectId);
        Assert.Equal(gradeDtos[0].Grade, result[0].Grade);
        _unitOfWorkMock.Verify(u => u.StudentSubjects.GetByTeacherIdAsync(1), Times.Once());
        _mapperMock.Verify(m => m.Map<List<GradeDto>>(studentSubjects), Times.Once());
    }

    [Fact]
    public async Task GetGradesByTeacherIdAsync_NoGrades_ReturnsEmptyList()
    {
        var studentSubjects = new List<StudentSubject>();
        var gradeDtos = new List<GradeDto>();

        _unitOfWorkMock.Setup(u => u.StudentSubjects.GetByTeacherIdAsync(1)).ReturnsAsync(studentSubjects);
        _mapperMock.Setup(m => m.Map<List<GradeDto>>(studentSubjects)).Returns(gradeDtos);

        var result = await _teacherService.GetGradesByTeacherIdAsync(1);

        Assert.NotNull(result);
        Assert.Empty(result);
        _unitOfWorkMock.Verify(u => u.StudentSubjects.GetByTeacherIdAsync(1), Times.Once());
    }

    [Fact]
    public async Task GetSubjectsByTeacherIdAsync_ReturnsSubjectDtos()
    {
        var subjects = new List<Subject> { new Subject { Id = 1, Name = "Math", TeacherId = 1 } };
        var subjectDtos = new List<SubjectDto> { new SubjectDto { Id = 1, Name = "Math" } };

        _unitOfWorkMock.Setup(u => u.Subjects.GetByTeacherIdAsync(1)).ReturnsAsync(subjects);
        _mapperMock.Setup(m => m.Map<List<SubjectDto>>(subjects)).Returns(subjectDtos);

        var result = await _teacherService.GetSubjectsByTeacherIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(subjectDtos.Count, result.Count);
        Assert.Equal(subjectDtos[0].Id, result[0].Id);
        Assert.Equal(subjectDtos[0].Name, result[0].Name);
        _unitOfWorkMock.Verify(u => u.Subjects.GetByTeacherIdAsync(1), Times.Once());
        _mapperMock.Verify(m => m.Map<List<SubjectDto>>(subjects), Times.Once());
    }

    [Fact]
    public async Task GetSubjectsByTeacherIdAsync_NoSubjects_ReturnsEmptyList()
    {
        var subjects = new List<Subject>();
        var subjectDtos = new List<SubjectDto>();

        _unitOfWorkMock.Setup(u => u.Subjects.GetByTeacherIdAsync(1)).ReturnsAsync(subjects);
        _mapperMock.Setup(m => m.Map<List<SubjectDto>>(subjects)).Returns(subjectDtos);

        var result = await _teacherService.GetSubjectsByTeacherIdAsync(1);

        Assert.NotNull(result);
        Assert.Empty(result);
        _unitOfWorkMock.Verify(u => u.Subjects.GetByTeacherIdAsync(1), Times.Once());
    }
}