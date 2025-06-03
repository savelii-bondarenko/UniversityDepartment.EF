using ElectronicDepartment.BLL.DTOs;

namespace ElectronicDepartment.BLL.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(string email, string password);
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
    Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
}
