using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using AutoMapper;
using ElectronicDepartment.BLL.DTOs;
using ElectronicDepartment.BLL.Services.Interfaces;
using ElectronicDepartment.DAL.UnitOfWork.Interfaces;
using ElectronicDepartment.Common.Configuration;
using System.Security.Cryptography;

namespace ElectronicDepartment.BLL.Services;

public class AuthService(IUnitOfWork unitOfWork, IMapper mapper, IOptions<JwtSettings> jwtSettings)
    : IAuthService
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    public async Task<AuthResponseDto> LoginAsync(string email, string password)
    {
        UserDto? user = null;

        var student = await unitOfWork.Students.GetByEmailAsync(email);
        if (student != null && VerifyPassword(password, student.PasswordHash))
        {
            user = mapper.Map<UserDto>(student);
        }

        if (user == null)
        {
            var teacher = await unitOfWork.Teachers.GetByEmailAsync(email);
            if (teacher != null && VerifyPassword(password, teacher.PasswordHash))
            {
                user = mapper.Map<UserDto>(teacher);
            }
        }

        if (user == null)
        {
            var manager = await unitOfWork.Managers.GetByEmailAsync(email);
            if (manager != null && VerifyPassword(password, manager.PasswordHash))
            {
                user = mapper.Map<UserDto>(manager);
            }
        }

        if (user == null)
        {
            var admin = await unitOfWork.Admins.GetByEmailAsync(email);
            if (admin != null && VerifyPassword(password, admin.PasswordHash))
            {
                user = mapper.Map<UserDto>(admin);
            }
        }

        if (user == null)
        {
            throw new Common.Exceptions.UnauthorizedAccessException("Невірний email або пароль");
        }

        var token = GenerateJwtToken(user);

        return new AuthResponseDto
        {
            Token = token,
            User = user
        };
    }

    private string GenerateJwtToken(UserDto user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        var hashedInput = HashPassword(password);
        return hashedInput == hashedPassword;
    }

    public async Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
    {
        throw new NotImplementedException();
    }
}