using AutoMapper;
using ElectronicDepartment.BLL.DTOs;
using ElectronicDepartment.Common.Configuration;
using ElectronicDepartment.DAL.Entities;
using ElectronicDepartment.DAL.UnitOfWork.Interfaces;
using Microsoft.Extensions.Options;
using Moq;
// Додано псевдонім, щоб уникнути конфлікту імен
using AppUnauthorizedAccessException = ElectronicDepartment.Common.Exceptions.UnauthorizedAccessException;

namespace ElectronicDepartment.Tests.Tests;

public class AuthServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IOptions<JwtSettings>> _jwtSettingsMock;
    private readonly ElectronicDepartment.BLL.Services.AuthService _authService;

    public AuthServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _jwtSettingsMock = new Mock<IOptions<JwtSettings>>();
        _jwtSettingsMock.Setup(j => j.Value).Returns(new JwtSettings
        {
            SecretKey = "verysecretkey1234567890abcdefghi",
            Issuer = "test_issuer",
            Audience = "test_audience",
            ExpirationInMinutes = 60
        });
        _authService = new ElectronicDepartment.BLL.Services.AuthService(_unitOfWorkMock.Object, _mapperMock.Object, _jwtSettingsMock.Object);
    }

    [Fact]
    public async Task LoginAsync_ValidStudentCredentials_ReturnsAuthResponse()
    {
        var student = new Student
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            PasswordHash = _authService.HashPassword("password"),
            Role = Common.Enums.UserRole.Student
        };

        var userDto = new UserDto
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Role = Common.Enums.UserRole.Student
        };

        _unitOfWorkMock.Setup(u => u.Students.GetByEmailAsync("john.doe@example.com")).ReturnsAsync(student);
        _mapperMock.Setup(m => m.Map<UserDto>(student)).Returns(userDto);

        var result = await _authService.LoginAsync("john.doe@example.com", "password");

        Assert.NotNull(result);
        Assert.NotNull(result.Token);
        Assert.Equal(userDto, result.User);
        _unitOfWorkMock.Verify(u => u.Students.GetByEmailAsync("john.doe@example.com"), Times.Once());
    }

    [Fact]
    public async Task LoginAsync_InvalidCredentials_ThrowsUnauthorizedAccessException()
    {
        _unitOfWorkMock.Setup(u => u.Students.GetByEmailAsync("john.doe@example.com")).ReturnsAsync((Student)null);
        _unitOfWorkMock.Setup(u => u.Teachers.GetByEmailAsync("john.doe@example.com")).ReturnsAsync((Teacher)null);
        _unitOfWorkMock.Setup(u => u.Managers.GetByEmailAsync("john.doe@example.com")).ReturnsAsync((Manager)null);
        _unitOfWorkMock.Setup(u => u.Admins.GetByEmailAsync("john.doe@example.com")).ReturnsAsync((Admin)null);

        await Assert.ThrowsAsync<AppUnauthorizedAccessException>(() =>
            _authService.LoginAsync("john.doe@example.com", "password"));
    }

    [Fact]
    public void HashPassword_ValidPassword_ReturnsHashedString()
    {
        var password = "password";

        var result = _authService.HashPassword(password);

        Assert.NotNull(result);
        Assert.NotEqual(password, result);
    }

    [Fact]
    public void VerifyPassword_CorrectPassword_ReturnsTrue()
    {
        var password = "password";
        var hashedPassword = _authService.HashPassword(password);

        var result = _authService.VerifyPassword(password, hashedPassword);

        Assert.True(result);
    }

    [Fact]
    public void VerifyPassword_IncorrectPassword_ReturnsFalse()
    {
        var password = "password";
        var hashedPassword = _authService.HashPassword("correct_password");

        var result = _authService.VerifyPassword(password, hashedPassword);

        Assert.False(result);
    }

    [Fact]
    public async Task ChangePasswordAsync_NotImplemented_ThrowsNotImplementedException()
    {
        await Assert.ThrowsAsync<NotImplementedException>(() =>
            _authService.ChangePasswordAsync(1, "old", "new"));
    }
}