using AutoMapper;
using ElectronicDepartment.BLL.DTOs;
using ElectronicDepartment.Common.Exceptions;
using ElectronicDepartment.DAL.Entities;
using ElectronicDepartment.DAL.UnitOfWork.Interfaces;
using Moq;

namespace ElectronicDepartment.Tests.Tests;

public class ManagerServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly BLL.Services.ManagerService _managerService;

    public ManagerServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _managerService = new BLL.Services.ManagerService(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsListOfManagerDtos()
    {
        var managers = new List<Manager> { new Manager { Id = 1, FirstName = "John", LastName = "Doe" } };
        var managerDtos = new List<ManagerDto> { new ManagerDto { Id = 1, FirstName = "John", LastName = "Doe" } };

        _unitOfWorkMock.Setup(u => u.Managers.GetAllAsync()).ReturnsAsync(managers);
        _mapperMock.Setup(m => m.Map<List<ManagerDto>>(managers)).Returns(managerDtos);

        var result = await _managerService.GetAllAsync();

        Assert.NotNull(result);
        Assert.Equal(managerDtos, result);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsManagerDto()
    {
        var manager = new Manager { Id = 1, FirstName = "John", LastName = "Doe" };
        var managerDto = new ManagerDto { Id = 1, FirstName = "John", LastName = "Doe" };

        _unitOfWorkMock.Setup(u => u.Managers.GetByIdAsync(1)).ReturnsAsync(manager);
        _mapperMock.Setup(m => m.Map<ManagerDto>(manager)).Returns(managerDto);

        var result = await _managerService.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(managerDto, result);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        _unitOfWorkMock.Setup(u => u.Managers.GetByIdAsync(1)).ReturnsAsync((Manager)null);

        var result = await _managerService.GetByIdAsync(1);

        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_ValidDto_AddsManager()
    {
        var createDto = new CreateManagerDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Password = "password"
        };

        var manager = new Manager
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com"
        };

        var managerDto = new ManagerDto
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com"
        };

        _unitOfWorkMock.Setup(u => u.Managers.EmailExistsAsync(createDto.Email)).ReturnsAsync(false);
        _mapperMock.Setup(m => m.Map<Manager>(createDto)).Returns(manager);
        _mapperMock.Setup(m => m.Map<ManagerDto>(manager)).Returns(managerDto);
        _unitOfWorkMock.Setup(u => u.Managers.AddAsync(manager)).Returns(Task.FromResult(0));
        _unitOfWorkMock.Setup(u => u.SaveAsync()).Returns(Task.FromResult(0));

        var result = await _managerService.AddAsync(createDto);

        Assert.NotNull(result);
        Assert.Equal(managerDto, result);
    }

    [Fact]
    public async Task AddAsync_EmptyEmail_ThrowsValidationException()
    {
        var createDto = new CreateManagerDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "",
            Password = "password"
        };

        await Assert.ThrowsAsync<ValidationException>(() => _managerService.AddAsync(createDto));
    }

    [Fact]
    public async Task UpdateAsync_ValidDto_UpdatesManager()
    {
        var updateDto = new UpdateManagerDto
        {
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane.doe@example.com"
        };

        var manager = new Manager
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe"
        };

        _unitOfWorkMock.Setup(u => u.Managers.GetByIdAsync(1)).ReturnsAsync(manager);
        _unitOfWorkMock.Setup(u => u.Managers.Update(manager)).Verifiable();
        _unitOfWorkMock.Setup(u => u.SaveAsync()).Returns(Task.FromResult(0));

        await _managerService.UpdateAsync(1, updateDto);

        _unitOfWorkMock.Verify(u => u.Managers.Update(manager), Times.Once());
    }

    [Fact]
    public async Task DeleteAsync_ExistingId_DeletesManager()
    {
        var manager = new Manager { Id = 1, FirstName = "John", LastName = "Doe" };

        _unitOfWorkMock.Setup(u => u.Managers.GetByIdAsync(1)).ReturnsAsync(manager);
        _unitOfWorkMock.Setup(u => u.Managers.Remove(manager)).Verifiable();
        _unitOfWorkMock.Setup(u => u.SaveAsync()).Returns(Task.FromResult(0));

        await _managerService.DeleteAsync(1);

        _unitOfWorkMock.Verify(u => u.Managers.Remove(manager), Times.Once());
    }
}