using AutoMapper;
using ElectronicDepartment.BLL.DTOs;
using ElectronicDepartment.Common.Exceptions;
using ElectronicDepartment.DAL.Entities;
using ElectronicDepartment.DAL.UnitOfWork.Interfaces;
using Moq;

namespace ElectronicDepartment.Tests.Tests;

public class DepartmentServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly BLL.Services.DepartmentService _departmentService;

    public DepartmentServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _departmentService = new BLL.Services.DepartmentService(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsListOfDepartmentDtos()
    {
        var departments = new List<Department> { new Department { Id = 1, Name = "CS" } };
        var departmentDtos = new List<DepartmentDto> { new DepartmentDto { Id = 1, Name = "CS" } };

        _unitOfWorkMock.Setup(u => u.Departments.GetAllAsync()).ReturnsAsync(departments);
        _mapperMock.Setup(m => m.Map<List<DepartmentDto>>(departments)).Returns(departmentDtos);

        var result = await _departmentService.GetAllAsync();

        Assert.NotNull(result);
        Assert.Equal(departmentDtos, result);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsDepartmentDto()
    {
        var department = new Department { Id = 1, Name = "CS" };
        var departmentDto = new DepartmentDto { Id = 1, Name = "CS" };

        _unitOfWorkMock.Setup(u => u.Departments.GetByIdAsync(1)).ReturnsAsync(department);
        _mapperMock.Setup(m => m.Map<DepartmentDto>(department)).Returns(departmentDto);

        var result = await _departmentService.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(departmentDto, result);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        _unitOfWorkMock.Setup(u => u.Departments.GetByIdAsync(1)).ReturnsAsync((Department)null);

        var result = await _departmentService.GetByIdAsync(1);

        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_ValidDto_AddsDepartment()
    {
        var createDto = new CreateDepartmentDto { Name = "CS" };
        var department = new Department { Id = 1, Name = "CS" };
        var departmentDto = new DepartmentDto { Id = 1, Name = "CS" };

        _mapperMock.Setup(m => m.Map<Department>(createDto)).Returns(department);
        _mapperMock.Setup(m => m.Map<DepartmentDto>(department)).Returns(departmentDto);
        _unitOfWorkMock.Setup(u => u.Departments.AddAsync(department)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveAsync()).ReturnsAsync(1); 

        var result = await _departmentService.AddAsync(createDto);

        Assert.NotNull(result);
        Assert.Equal(departmentDto, result);
    }

    [Fact]
    public async Task AddAsync_EmptyName_ThrowsValidationException()
    {
        var createDto = new CreateDepartmentDto { Name = "" };

        await Assert.ThrowsAsync<ValidationException>(() => _departmentService.AddAsync(createDto));
    }

    [Fact]
    public async Task UpdateAsync_ValidDto_UpdatesDepartment()
    {
        var updateDto = new UpdateDepartmentDto { Name = "CS Updated" };
        var department = new Department { Id = 1, Name = "CS" };

        _unitOfWorkMock.Setup(u => u.Departments.GetByIdAsync(1)).ReturnsAsync(department);
        _unitOfWorkMock.Setup(u => u.Departments.Update(department)).Verifiable();
        _unitOfWorkMock.Setup(u => u.SaveAsync()).ReturnsAsync(1); 

        await _departmentService.UpdateAsync(1, updateDto);

        _unitOfWorkMock.Verify(u => u.Departments.Update(department), Times.Once());
    }

    [Fact]
    public async Task UpdateAsync_NonExistingId_ThrowsEntityNotFoundException()
    {
        var updateDto = new UpdateDepartmentDto { Name = "CS Updated" };
        _unitOfWorkMock.Setup(u => u.Departments.GetByIdAsync(1)).ReturnsAsync((Department)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _departmentService.UpdateAsync(1, updateDto));
    }

    [Fact]
    public async Task DeleteAsync_ExistingId_DeletesDepartment()
    {
        var department = new Department { Id = 1, Name = "CS" };
        _unitOfWorkMock.Setup(u => u.Departments.GetByIdAsync(1)).ReturnsAsync(department);
        _unitOfWorkMock.Setup(u => u.Departments.Remove(department)).Verifiable();
        _unitOfWorkMock.Setup(u => u.SaveAsync()).ReturnsAsync(1);

        await _departmentService.DeleteAsync(1);

        _unitOfWorkMock.Verify(u => u.Departments.Remove(department), Times.Once());
    }

    [Fact]
    public async Task DeleteAsync_NonExistingId_DoesNothing()
    {
        _unitOfWorkMock.Setup(u => u.Departments.GetByIdAsync(1)).ReturnsAsync((Department)null);

        await _departmentService.DeleteAsync(1);

        _unitOfWorkMock.Verify(u => u.Departments.Remove(It.IsAny<Department>()), Times.Never());
    }

    [Fact]
    public async Task GetSubjectsByDepartmentIdAsync_ReturnsSubjectDtos()
    {
        var subjects = new List<Subject> { new Subject { Id = 1, Name = "Math" } };
        var subjectDtos = new List<SubjectDto> { new SubjectDto { Id = 1, Name = "Math" } };

        _unitOfWorkMock.Setup(u => u.Subjects.GetByDepartmentIdAsync(1)).ReturnsAsync(subjects);
        _mapperMock.Setup(m => m.Map<List<SubjectDto>>(subjects)).Returns(subjectDtos);

        var result = await _departmentService.GetSubjectsByDepartmentIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(subjectDtos, result);
    }

    [Fact]
    public async Task GetTeachersByDepartmentIdAsync_ReturnsTeacherDtos()
    {
        var teachers = new List<Teacher> { new Teacher { Id = 1, FirstName = "John" } };
        var teacherDtos = new List<TeacherDto> { new TeacherDto { Id = 1, FirstName = "John" } };

        _unitOfWorkMock.Setup(u => u.Teachers.GetTeachersByDepartmentIdAsync(1)).ReturnsAsync(teachers);
        _mapperMock.Setup(m => m.Map<List<TeacherDto>>(teachers)).Returns(teacherDtos);

        var result = await _departmentService.GetTeachersByDepartmentIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(teacherDtos, result);
    }

    [Fact]
    public async Task GetGroupsByDepartmentIdAsync_ReturnsGroupDtos()
    {
        var groups = new List<Group> { new Group { Id = 1, Name = "Group A" } };
        var groupDtos = new List<GroupDto> { new GroupDto { Id = 1, Name = "Group A" } };

        _unitOfWorkMock.Setup(u => u.Groups.GetByDepartmentIdAsync(1)).ReturnsAsync(groups);
        _mapperMock.Setup(m => m.Map<List<GroupDto>>(groups)).Returns(groupDtos);

        var result = await _departmentService.GetGroupsByDepartmentIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(groupDtos, result);
    }
}