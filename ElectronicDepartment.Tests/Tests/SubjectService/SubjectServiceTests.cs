using AutoMapper;
using ElectronicDepartment.BLL.DTOs;
using ElectronicDepartment.Common.Exceptions;
using ElectronicDepartment.DAL.Entities;
using ElectronicDepartment.DAL.UnitOfWork.Interfaces;
using Moq;

namespace ElectronicDepartment.Tests.Tests;

public class SubjectServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly BLL.Services.SubjectService _subjectService;

    public SubjectServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _subjectService = new BLL.Services.SubjectService(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsListOfSubjectDtos()
    {
        var subjects = new List<Subject> { new Subject { Id = 1, Name = "Math" } };
        var subjectDtos = new List<SubjectDto> { new SubjectDto { Id = 1, Name = "Math" } };

        _unitOfWorkMock.Setup(u => u.Subjects.GetAllAsync()).ReturnsAsync(subjects);
        _mapperMock.Setup(m => m.Map<List<SubjectDto>>(subjects)).Returns(subjectDtos);

        var result = await _subjectService.GetAllAsync();

        Assert.NotNull(result);
        Assert.Equal(subjectDtos, result);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsSubjectDto()
    {
        var subject = new Subject { Id = 1, Name = "Math" };
        var subjectDto = new SubjectDto { Id = 1, Name = "Math" };

        _unitOfWorkMock.Setup(u => u.Subjects.GetByIdAsync(1)).ReturnsAsync(subject);
        _mapperMock.Setup(m => m.Map<SubjectDto>(subject)).Returns(subjectDto);

        var result = await _subjectService.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(subjectDto, result);
    }

    [Fact]
    public async Task AddAsync_ValidDto_AddsSubject()
    {
        var createDto = new CreateSubjectDto { Name = "Math", DepartmentId = 1, TeacherId = 1, GroupId = 1 };
        var teacher = new Teacher { Id = 1 };
        var subject = new Subject { Id = 1, Name = "Math" };
        var subjectDto = new SubjectDto { Id = 1, Name = "Math" };

        _unitOfWorkMock.Setup(u => u.Teachers.GetByIdAsync(1)).ReturnsAsync(teacher);
        _mapperMock.Setup(m => m.Map<Subject>(createDto)).Returns(subject);
        _mapperMock.Setup(m => m.Map<SubjectDto>(subject)).Returns(subjectDto);
        _unitOfWorkMock.Setup(u => u.Subjects.AddAsync(subject)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveAsync()).ReturnsAsync(1); 

        var result = await _subjectService.AddAsync(createDto);

        Assert.NotNull(result);
        Assert.Equal(subjectDto, result);
    }

    [Fact]
    public async Task AddAsync_EmptyName_ThrowsValidationException()
    {
        var createDto = new CreateSubjectDto { Name = "", DepartmentId = 1, TeacherId = 1, GroupId = 1 };

        await Assert.ThrowsAsync<ValidationException>(() => _subjectService.AddAsync(createDto));
    }

    [Fact]
    public async Task UpdateAsync_ValidDto_UpdatesSubject()
    {
        var updateDto = new UpdateSubjectDto { Id = 1, Name = "Math Updated", DepartmentId = 1, TeacherId = 1, GroupId = 1 };
        var subject = new Subject { Id = 1, Name = "Math" };
        var teacher = new Teacher { Id = 1 };

        _unitOfWorkMock.Setup(u => u.Subjects.GetByIdAsync(1)).ReturnsAsync(subject);
        _unitOfWorkMock.Setup(u => u.Teachers.GetByIdAsync(1)).ReturnsAsync(teacher);
        _unitOfWorkMock.Setup(u => u.Subjects.Update(subject)).Verifiable();
        _unitOfWorkMock.Setup(u => u.SaveAsync()).ReturnsAsync(1);

        await _subjectService.UpdateAsync(updateDto);

        _unitOfWorkMock.Verify(u => u.Subjects.Update(subject), Times.Once());
    }

    [Fact]
    public async Task DeleteAsync_ExistingId_DeletesSubject()
    {
        var subject = new Subject { Id = 1, Name = "Math" };
        _unitOfWorkMock.Setup(u => u.Subjects.GetByIdAsync(1)).ReturnsAsync(subject);
        _unitOfWorkMock.Setup(u => u.Subjects.Remove(subject)).Verifiable();
        _unitOfWorkMock.Setup(u => u.SaveAsync()).ReturnsAsync(1);

        await _subjectService.DeleteAsync(1);

        _unitOfWorkMock.Verify(u => u.Subjects.Remove(subject), Times.Once());
    }
}