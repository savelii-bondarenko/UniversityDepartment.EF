using AutoMapper;
using ElectronicDepartment.BLL.DTOs;
using ElectronicDepartment.BLL.Services.Interfaces;
using ElectronicDepartment.Common.Exceptions;
using ElectronicDepartment.DAL.Entities;
using ElectronicDepartment.DAL.UnitOfWork.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ElectronicDepartment.BLL.Services;

public class GroupService(IUnitOfWork unitOfWork, IMapper mapper) : IGroupService
{
    public async Task<GroupDto> AddAsync(CreateGroupDto createDto)
    {
        if (string.IsNullOrWhiteSpace(createDto.Name))
            throw new ValidationException("Назва групи не може бути порожньою.");

        var department = await unitOfWork.Departments.GetByIdAsync(createDto.DepartmentId);
        if (department == null)
            throw new EntityNotFoundException("Department", createDto.DepartmentId);

        if (await unitOfWork.Groups.NameExistsInDepartmentAsync(createDto.Name, createDto.DepartmentId))
            throw new ValidationException("Група з такою назвою вже існує в цій кафедрі.");

        var group = mapper.Map<Group>(createDto);

        await unitOfWork.BeginTransactionAsync();
        try
        {
            await unitOfWork.Groups.AddAsync(group);
            await unitOfWork.SaveAsync();
            await unitOfWork.CommitTransactionAsync();

            return mapper.Map<GroupDto>(group);
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<GroupDto?> GetByIdAsync(int id)
    {
        var group = await unitOfWork.Groups.GetByIdAsync(id);
        if (group == null)
            return null;

        return mapper.Map<GroupDto>(group);
    }

    public async Task<List<GroupDto>> GetAllAsync()
    {
        var groups = await unitOfWork.Groups.GetAllAsync();
        return mapper.Map<List<GroupDto>>(groups);
    }

    public async Task<List<GroupDto>> GetFilteredAsync(GroupFilterDto filter)
    {
        var query = unitOfWork.Groups.GetAllQueryable();

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var searchTerm = filter.SearchTerm.ToLower();
            query = query.Where(g => g.Name.ToLower().Contains(searchTerm));
        }

        if (filter.DepartmentId.HasValue)
        {
            query = query.Where(g => g.DepartmentId == filter.DepartmentId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.SortBy))
        {
            query = filter.SortBy switch
            {
                "Name" => filter.SortDescending ? query.OrderByDescending(g => g.Name) : query.OrderBy(g => g.Name),
                "DepartmentId" => filter.SortDescending ? query.OrderByDescending(g => g.DepartmentId) : query.OrderBy(g => g.DepartmentId),
                _ => query.OrderBy(g => g.Name)
            };
        }
        else
        {
            query = query.OrderBy(g => g.Name);
        }

        var groups = await query.ToListAsync();
        return mapper.Map<List<GroupDto>>(groups);
    }

    public async Task UpdateAsync(int id, UpdateGroupDto updateDto)
    {
        var group = await unitOfWork.Groups.GetByIdAsync(id);
        if (group == null)
            throw new EntityNotFoundException("Group", id);

        var department = await unitOfWork.Departments.GetByIdAsync(updateDto.DepartmentId);
        if (department == null)
            throw new EntityNotFoundException("Department", updateDto.DepartmentId);

        if (await unitOfWork.Groups.NameExistsInDepartmentAsync(updateDto.Name, updateDto.DepartmentId, id))
            throw new ValidationException("Група з такою назвою вже існує в цій кафедрі.");

        await unitOfWork.BeginTransactionAsync();
        try
        {
            mapper.Map(updateDto, group);
            unitOfWork.Groups.Update(group);
            await unitOfWork.SaveAsync();
            await unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task DeleteAsync(int id)
    {
        var group = await unitOfWork.Groups.GetByIdAsync(id);
        if (group == null)
        {
            return;
        }

        if (group.Students.Any() || group.Subjects.Any())
        {
            throw new ValidationException("Неможливо видалити групу, яка має студентів або предмети.");
        }

        await unitOfWork.BeginTransactionAsync();
        try
        {
            unitOfWork.Groups.Remove(group);
            await unitOfWork.SaveAsync();
            await unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
