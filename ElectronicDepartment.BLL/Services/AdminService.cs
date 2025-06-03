using ElectronicDepartment.BLL.DTOs;
using ElectronicDepartment.BLL.Services.Interfaces;
using ElectronicDepartment.Common.Exceptions;
using ElectronicDepartment.DAL.Entities;
using ElectronicDepartment.DAL.UnitOfWork.Interfaces;
using AutoMapper;

namespace ElectronicDepartment.BLL.Services;

public class AdminService(IUnitOfWork unitOfWork, IMapper mapper) : IAdminService
{
    public async Task<List<AdminDto>> GetAllAsync()
    {
        var admins = await unitOfWork.Admins.GetAllAsync();
        return mapper.Map<List<AdminDto>>(admins);
    }

    public async Task<AdminDto?> GetByIdAsync(int id)
    {
        var admin = await unitOfWork.Admins.GetByIdAsync(id);
        return admin == null ? null : mapper.Map<AdminDto>(admin);
    }

    public async Task<AdminDto> AddAsync(CreateAdminDto createDto)
    {
        if (string.IsNullOrWhiteSpace(createDto.Email))
            throw new ValidationException("Email не може бути порожнім");

        if (await unitOfWork.Admins.EmailExistsAsync(createDto.Email))
            throw new ValidationException("Адміністратор з таким email вже існує");

        var admin = mapper.Map<Admin>(createDto);
        admin.PasswordHash = HashPassword(createDto.Password);
        admin.Role = Common.Enums.UserRole.Admin;

        await unitOfWork.Admins.AddAsync(admin);
        await unitOfWork.SaveAsync();

        return mapper.Map<AdminDto>(admin);
    }

    public async Task UpdateAsync(int id, UpdateAdminDto updateDto)
    {
        var admin = await unitOfWork.Admins.GetByIdAsync(id);
        if (admin == null)
            throw new EntityNotFoundException("Admin", id);

        mapper.Map(updateDto, admin);
        unitOfWork.Admins.Update(admin);
        await unitOfWork.SaveAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var admin = await unitOfWork.Admins.GetByIdAsync(id);
        if (admin == null)
            return;

        unitOfWork.Admins.Remove(admin);
        await unitOfWork.SaveAsync();
    }

    private string HashPassword(string password)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}
