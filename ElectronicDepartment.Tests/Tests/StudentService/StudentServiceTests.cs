using AutoMapper;
using ElectronicDepartment.BLL.DTOs;
using ElectronicDepartment.Common.Exceptions;
using ElectronicDepartment.DAL.Entities;
using ElectronicDepartment.DAL.UnitOfWork.Interfaces;
using Moq;

namespace ElectronicDepartment.Tests.Tests;

public class StudentServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly BLL.Services.StudentService _studentService;

    public StudentServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _studentService = new BLL.Services.StudentService(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsListOfStudentDtos()
    {
        var students = new List<Student> { new Student { Id = 1, FirstName = "John", LastName = "Doe" } };
        var studentDtos = new List<StudentDto> { new StudentDto { Id = 1, FirstName = "John", LastName = "Doe" } };

        _unitOfWorkMock.Setup(u => u.Students.GetAllAsync()).ReturnsAsync(students);
        _mapperMock.Setup(m => m.Map<List<StudentDto>>(students)).Returns(studentDtos);

        var result = await _studentService.GetAllAsync();

        Assert.NotNull(result);
        Assert.Equal(studentDtos, result);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsStudentDto()
    {
        var student = new Student { Id = 1, FirstName = "John", LastName = "Doe" };
        var studentDto = new StudentDto { Id = 1, FirstName = "John", LastName = "Doe" };

        _unitOfWorkMock.Setup(u => u.Students.GetByIdAsync(1)).ReturnsAsync(student);
        _mapperMock.Setup(m => m.Map<StudentDto>(student)).Returns(studentDto);

        var result = await _studentService.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(studentDto, result);
    }

    [Fact]
    public async Task AddAsync_ValidDto_AddsStudent()
    {
        var createDto = new CreateStudentDto { FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", Password = "password", GroupId = 1 };
        var group = new Group { Id = 1 };
        var student = new Student { Id = 1, FirstName = "John", LastName = "Doe" };
        var studentDto = new StudentDto { Id = 1, FirstName = "John", LastName = "Doe" };

        _unitOfWorkMock.Setup(u => u.Students.EmailExistsAsync(createDto.Email)).ReturnsAsync(false);
        _unitOfWorkMock.Setup(u => u.Groups.GetByIdAsync(1)).ReturnsAsync(group);
        _mapperMock.Setup(m => m.Map<Student>(createDto)).Returns(student);
        _mapperMock.Setup(m => m.Map<StudentDto>(student)).Returns(studentDto);
        _unitOfWorkMock.Setup(u => u.Students.AddAsync(student)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveAsync()).ReturnsAsync(1);

        var result = await _studentService.AddAsync(createDto);

        Assert.NotNull(result);
        Assert.Equal(studentDto, result);
    }

    [Fact]
    public async Task AddAsync_EmptyEmail_ThrowsValidationException()
    {
        var createDto = new CreateStudentDto { FirstName = "John", LastName = "Doe", Email = "", Password = "password", GroupId = 1 };

        await Assert.ThrowsAsync<ValidationException>(() => _studentService.AddAsync(createDto));
    }

    [Fact]
    public async Task UpdateAsync_ValidDto_UpdatesStudent()
    {
        var updateDto = new UpdateStudentDto { FirstName = "Jane", LastName = "Doe", Email = "jane.doe@example.com", GroupId = 1 };
        var student = new Student { Id = 1, FirstName = "John", LastName = "Doe" };
        var group = new Group { Id = 1 };

        _unitOfWorkMock.Setup(u => u.Students.GetByIdAsync(1)).ReturnsAsync(student);
        _unitOfWorkMock.Setup(u => u.Groups.GetByIdAsync(1)).ReturnsAsync(group);
        _unitOfWorkMock.Setup(u => u.Students.Update(student)).Verifiable();
        _unitOfWorkMock.Setup(u => u.SaveAsync()).ReturnsAsync(1);

        await _studentService.UpdateAsync(1, updateDto);

        _unitOfWorkMock.Verify(u => u.Students.Update(student), Times.Once());
    }

    [Fact]
    public async Task DeleteAsync_ExistingId_DeletesStudent()
    {
        var student = new Student { Id = 1, FirstName = "John", LastName = "Doe" };
        _unitOfWorkMock.Setup(u => u.Students.GetByIdAsync(1)).ReturnsAsync(student);
        _unitOfWorkMock.Setup(u => u.Students.Remove(student)).Verifiable();
        _unitOfWorkMock.Setup(u => u.SaveAsync()).ReturnsAsync(1);

        await _studentService.DeleteAsync(1);

        _unitOfWorkMock.Verify(u => u.Students.Remove(student), Times.Once());
    }

    [Fact]
    public async Task GetSubjectsByStudentIdAsync_ReturnsSubjectDtos()
    {
        var studentSubjects = new List<StudentSubject> {
            new StudentSubject {
                StudentId = 1,
                SubjectId = 1,
                Subject = new Subject { Id = 1, Name = "Math" }
            }
        };
        var subjectDtos = new List<SubjectDto> { new SubjectDto { Id = 1, Name = "Math" } };

        _unitOfWorkMock.Setup(u => u.StudentSubjects.GetByStudentIdAsync(1)).ReturnsAsync(studentSubjects);
        _mapperMock.Setup(m => m.Map<List<SubjectDto>>(It.IsAny<List<Subject>>())).Returns(subjectDtos);

        var result = await _studentService.GetSubjectsByStudentIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(subjectDtos, result);
    }
}