using AutoMapper;
using ElectronicDepartment.BLL.DTOs;
using ElectronicDepartment.Common.Exceptions;
using ElectronicDepartment.DAL.Entities;
using ElectronicDepartment.DAL.UnitOfWork.Interfaces;
using Moq;

namespace ElectronicDepartment.Tests.Tests;

public class AdminServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly BLL.Services.AdminService _adminService;

    public AdminServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _adminService = new BLL.Services.AdminService(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsListOfAdminDtos()
    {
        var admins = new List<Admin>
        {
            new Admin { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" }
        };
        var adminDtos = new List<AdminDto>
        {
            new AdminDto { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" }
        };

        _unitOfWorkMock.Setup(u => u.Admins.GetAllAsync()).ReturnsAsync(admins);
        _mapperMock.Setup(m => m.Map<List<AdminDto>>(admins)).Returns(adminDtos);

        var result = await _adminService.GetAllAsync();

        Assert.NotNull(result);
        Assert.Equal(adminDtos, result);
        _unitOfWorkMock.Verify(u => u.Admins.GetAllAsync(), Times.Once());
        _mapperMock.Verify(m => m.Map<List<AdminDto>>(admins), Times.Once());
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsAdminDto()
    {
        var admin = new Admin { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" };
        var adminDto = new AdminDto { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" };

        _unitOfWorkMock.Setup(u => u.Admins.GetByIdAsync(1)).ReturnsAsync(admin);
        _mapperMock.Setup(m => m.Map<AdminDto>(admin)).Returns(adminDto);

        var result = await _adminService.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(adminDto, result);
        _unitOfWorkMock.Verify(u => u.Admins.GetByIdAsync(1), Times.Once());
        _mapperMock.Verify(m => m.Map<AdminDto>(admin), Times.Once());
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        _unitOfWorkMock.Setup(u => u.Admins.GetByIdAsync(1)).ReturnsAsync((Admin)null);

        var result = await _adminService.GetByIdAsync(1);

        Assert.Null(result);
        _unitOfWorkMock.Verify(u => u.Admins.GetByIdAsync(1), Times.Once());
    }

    [Fact]
    public async Task AddAsync_ValidDto_AddsAdminAndReturnsDto()
    {
        var createDto = new CreateAdminDto { FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", Password = "password" };
        var admin = new Admin { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", PasswordHash = "hashed_password", Role = Common.Enums.UserRole.Admin };
        var adminDto = new AdminDto { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" };

        _unitOfWorkMock.Setup(u => u.Admins.EmailExistsAsync(createDto.Email)).ReturnsAsync(false);
        _mapperMock.Setup(m => m.Map<Admin>(createDto)).Returns(admin);
        _mapperMock.Setup(m => m.Map<AdminDto>(admin)).Returns(adminDto);
        _unitOfWorkMock.Setup(u => u.Admins.AddAsync(admin)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveAsync()).ReturnsAsync(1);

        var result = await _adminService.AddAsync(createDto);

        Assert.NotNull(result);
        Assert.Equal(adminDto, result);
        _unitOfWorkMock.Verify(u => u.Admins.AddAsync(It.Is<Admin>(a => a.Role == Common.Enums.UserRole.Admin)), Times.Once());
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once());
        _mapperMock.Verify(m => m.Map<AdminDto>(admin), Times.Once());
    }

    [Fact]
    public async Task AddAsync_EmptyEmail_ThrowsValidationException()
    {
        var createDto = new CreateAdminDto { FirstName = "John", LastName = "Doe", Email = "", Password = "password" };

        await Assert.ThrowsAsync<ValidationException>(() => _adminService.AddAsync(createDto));
    }

    [Fact]
    public async Task AddAsync_ExistingEmail_ThrowsValidationException()
    {
        var createDto = new CreateAdminDto { FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", Password = "password" };
        _unitOfWorkMock.Setup(u => u.Admins.EmailExistsAsync(createDto.Email)).ReturnsAsync(true);

        await Assert.ThrowsAsync<ValidationException>(() => _adminService.AddAsync(createDto));
    }

    [Fact]
    public async Task UpdateAsync_ValidDto_UpdatesAdmin()
    {
        var updateDto = new UpdateAdminDto { FirstName = "Jane", LastName = "Doe", Email = "jane.doe@example.com" };
        var admin = new Admin { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" };

        _unitOfWorkMock.Setup(u => u.Admins.GetByIdAsync(1)).ReturnsAsync(admin);
        _unitOfWorkMock.Setup(u => u.Admins.Update(admin)).Verifiable();
        _unitOfWorkMock.Setup(u => u.SaveAsync()).ReturnsAsync(1);
        _mapperMock.Setup(m => m.Map(updateDto, admin)).Verifiable();

        await _adminService.UpdateAsync(1, updateDto);

        _unitOfWorkMock.Verify(u => u.Admins.Update(admin), Times.Once());
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once());
        _mapperMock.Verify(m => m.Map(updateDto, admin), Times.Once());
    }

    [Fact]
    public async Task UpdateAsync_NonExistingId_ThrowsEntityNotFoundException()
    {
        var updateDto = new UpdateAdminDto { FirstName = "Jane", LastName = "Doe", Email = "jane.doe@example.com" };
        _unitOfWorkMock.Setup(u => u.Admins.GetByIdAsync(1)).ReturnsAsync((Admin)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _adminService.UpdateAsync(1, updateDto));
    }

    [Fact]
    public async Task DeleteAsync_ExistingId_DeletesAdmin()
    {
        var admin = new Admin { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" };
        _unitOfWorkMock.Setup(u => u.Admins.GetByIdAsync(1)).ReturnsAsync(admin);
        _unitOfWorkMock.Setup(u => u.Admins.Remove(admin)).Verifiable();
        _unitOfWorkMock.Setup(u => u.SaveAsync()).ReturnsAsync(1);

        await _adminService.DeleteAsync(1);

        _unitOfWorkMock.Verify(u => u.Admins.Remove(admin), Times.Once());
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once());
    }

    [Fact]
    public async Task DeleteAsync_NonExistingId_DoesNothing()
    {
        _unitOfWorkMock.Setup(u => u.Admins.GetByIdAsync(1)).ReturnsAsync((Admin)null);

        await _adminService.DeleteAsync(1);

        _unitOfWorkMock.Verify(u => u.Admins.Remove(It.IsAny<Admin>()), Times.Never());
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Never());
    }
}