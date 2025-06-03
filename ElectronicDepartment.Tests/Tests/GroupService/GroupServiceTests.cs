using AutoMapper;
using ElectronicDepartment.BLL.DTOs;
using ElectronicDepartment.Common.Exceptions;
using ElectronicDepartment.DAL.Entities;
using ElectronicDepartment.DAL.UnitOfWork.Interfaces;
using Moq;

namespace ElectronicDepartment.Tests.Tests;

public class GroupServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly BLL.Services.GroupService _groupService;

    public GroupServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _groupService = new BLL.Services.GroupService(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task AddAsync_ValidDto_AddsGroup()
    {
        var createDto = new CreateGroupDto { Name = "Group A", DepartmentId = 1 };
        var department = new Department { Id = 1 };
        var group = new Group { Id = 1, Name = "Group A", DepartmentId = 1 };
        var groupDto = new GroupDto { Id = 1, Name = "Group A", DepartmentId = 1 };

        _unitOfWorkMock.Setup(u => u.Departments.GetByIdAsync(1)).ReturnsAsync(department);
        _unitOfWorkMock
            .Setup(u => u.Groups.NameExistsInDepartmentAsync("Group A", 1, null))
            .ReturnsAsync(false);

        _mapperMock.Setup(m => m.Map<Group>(createDto)).Returns(group);
        _mapperMock.Setup(m => m.Map<GroupDto>(group)).Returns(groupDto);
        _unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.Groups.AddAsync(group)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveAsync()).ReturnsAsync(1);
        _unitOfWorkMock.Setup(u => u.CommitTransactionAsync()).Returns(Task.CompletedTask);

        var result = await _groupService.AddAsync(createDto);

        Assert.NotNull(result);
        Assert.Equal(groupDto, result);
    }

    [Fact]
    public async Task AddAsync_EmptyName_ThrowsValidationException()
    {
        var createDto = new CreateGroupDto { Name = "", DepartmentId = 1 };
        await Assert.ThrowsAsync<ValidationException>(() => _groupService.AddAsync(createDto));
    }

    [Fact]
    public async Task AddAsync_NonExistingDepartment_ThrowsEntityNotFoundException()
    {
        var createDto = new CreateGroupDto { Name = "Group A", DepartmentId = 1 };
        _unitOfWorkMock.Setup(u => u.Departments.GetByIdAsync(1)).ReturnsAsync((Department)null);
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _groupService.AddAsync(createDto));
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsGroupDto()
    {
        var group = new Group { Id = 1, Name = "Group A" };
        var groupDto = new GroupDto { Id = 1, Name = "Group A" };

        _unitOfWorkMock.Setup(u => u.Groups.GetByIdAsync(1)).ReturnsAsync(group);
        _mapperMock.Setup(m => m.Map<GroupDto>(group)).Returns(groupDto);

        var result = await _groupService.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(groupDto, result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsGroupDtos()
    {
        var groups = new List<Group> { new Group { Id = 1, Name = "Group A" } };
        var groupDtos = new List<GroupDto> { new GroupDto { Id = 1, Name = "Group A" } };

        _unitOfWorkMock.Setup(u => u.Groups.GetAllAsync()).ReturnsAsync(groups);
        _mapperMock.Setup(m => m.Map<List<GroupDto>>(groups)).Returns(groupDtos);

        var result = await _groupService.GetAllAsync();

        Assert.NotNull(result);
        Assert.Equal(groupDtos, result);
    }

    [Fact]
    public async Task UpdateAsync_ValidDto_UpdatesGroup()
    {
        var updateDto = new UpdateGroupDto { Name = "Group B", DepartmentId = 1 };
        var group = new Group { Id = 1, Name = "Group A", DepartmentId = 1 };
        var department = new Department { Id = 1 };

        _unitOfWorkMock.Setup(u => u.Groups.GetByIdAsync(1)).ReturnsAsync(group);
        _unitOfWorkMock.Setup(u => u.Departments.GetByIdAsync(1)).ReturnsAsync(department);
        _unitOfWorkMock.Setup(u => u.Groups.NameExistsInDepartmentAsync("Group B", 1, 1)).ReturnsAsync(false);
        _unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.Groups.Update(group)).Verifiable();
        _unitOfWorkMock.Setup(u => u.SaveAsync()).ReturnsAsync(1);
        _unitOfWorkMock.Setup(u => u.CommitTransactionAsync()).Returns(Task.CompletedTask);

        await _groupService.UpdateAsync(1, updateDto);

        _unitOfWorkMock.Verify(u => u.Groups.Update(group), Times.Once());
    }

    [Fact]
    public async Task UpdateAsync_NonExistingGroup_ThrowsEntityNotFoundException()
    {
        var updateDto = new UpdateGroupDto { Name = "Group B", DepartmentId = 1 };
        _unitOfWorkMock.Setup(u => u.Groups.GetByIdAsync(1)).ReturnsAsync((Group)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _groupService.UpdateAsync(1, updateDto));
    }

    [Fact]
    public async Task DeleteAsync_EmptyGroup_DeletesGroup()
    {
        var group = new Group { Id = 1, Name = "Group A", Students = new List<Student>(), Subjects = new List<Subject>() };

        _unitOfWorkMock.Setup(u => u.Groups.GetByIdAsync(1)).ReturnsAsync(group);
        _unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.Groups.Remove(group)).Verifiable();
        _unitOfWorkMock.Setup(u => u.SaveAsync()).ReturnsAsync(1);
        _unitOfWorkMock.Setup(u => u.CommitTransactionAsync()).Returns(Task.CompletedTask);

        await _groupService.DeleteAsync(1);

        _unitOfWorkMock.Verify(u => u.Groups.Remove(group), Times.Once());
    }

    [Fact]
    public async Task DeleteAsync_GroupWithStudents_ThrowsValidationException()
    {
        var group = new Group { Id = 1, Name = "Group A", Students = new List<Student> { new Student() } };
        _unitOfWorkMock.Setup(u => u.Groups.GetByIdAsync(1)).ReturnsAsync(group);

        await Assert.ThrowsAsync<ValidationException>(() => _groupService.DeleteAsync(1));
    }
}