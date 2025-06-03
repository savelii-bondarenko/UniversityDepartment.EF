using ElectronicDepartment.BLL.DTOs;
using ElectronicDepartment.BLL.Services.Interfaces;
using ElectronicDepartment.Common.Exceptions;
using ElectronicDepartment.DAL.Entities;
using ElectronicDepartment.DAL.UnitOfWork.Interfaces;
using AutoMapper;

namespace ElectronicDepartment.BLL.Services;

public class ManagerService(IUnitOfWork unitOfWork, IMapper mapper) : IManagerService
{
    public async Task<List<ManagerDto>> GetAllAsync()
    {
        var managers = await unitOfWork.Managers.GetAllAsync();
        return mapper.Map<List<ManagerDto>>(managers);
    }

    public async Task<ManagerDto?> GetByIdAsync(int id)
    {
        var manager = await unitOfWork.Managers.GetByIdAsync(id);
        return manager == null ? null : mapper.Map<ManagerDto>(manager);
    }

    public async Task<ManagerDto> AddAsync(CreateManagerDto createDto)
    {
        if (string.IsNullOrWhiteSpace(createDto.Email))
            throw new ValidationException("Email не може бути порожнім");

        if (await unitOfWork.Managers.EmailExistsAsync(createDto.Email))
            throw new ValidationException("Менеджер з таким email вже існує");

        var manager = mapper.Map<Manager>(createDto);
        manager.PasswordHash = HashPassword(createDto.Password);
        manager.Role = Common.Enums.UserRole.Manager;

        await unitOfWork.Managers.AddAsync(manager);
        await unitOfWork.SaveAsync();

        return mapper.Map<ManagerDto>(manager);
    }

    public async Task UpdateAsync(int id, UpdateManagerDto updateDto)
    {
        var manager = await unitOfWork.Managers.GetByIdAsync(id);
        if (manager == null)
            throw new EntityNotFoundException("Manager", id);

        mapper.Map(updateDto, manager);
        unitOfWork.Managers.Update(manager);
        await unitOfWork.SaveAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var manager = await unitOfWork.Managers.GetByIdAsync(id);
        if (manager == null)
            return;

        unitOfWork.Managers.Remove(manager);
        await unitOfWork.SaveAsync();
    }

    private string HashPassword(string password)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}
