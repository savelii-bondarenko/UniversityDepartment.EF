using AutoMapper;
using ElectronicDepartment.BLL.DTOs;
using ElectronicDepartment.BLL.Services.Interfaces;
using ElectronicDepartment.Common.Exceptions;
using ElectronicDepartment.DAL.Entities;
using ElectronicDepartment.DAL.UnitOfWork.Interfaces;

namespace ElectronicDepartment.BLL.Services;

public class DepartmentService(IUnitOfWork unitOfWork, IMapper mapper) : IDepartmentService
{
    public async Task<List<DepartmentDto>> GetAllAsync()
    {
        var departments = await unitOfWork.Departments.GetAllAsync();
        return mapper.Map<List<DepartmentDto>>(departments);
    }

    public async Task<DepartmentDto?> GetByIdAsync(int id)
    {
        var department = await unitOfWork.Departments.GetByIdAsync(id);
        return department == null ? null : mapper.Map<DepartmentDto>(department);
    }

    public async Task<DepartmentDto> AddAsync(CreateDepartmentDto createDto)
    {
        if (string.IsNullOrWhiteSpace(createDto.Name))
            throw new ValidationException("Назва кафедри не може бути порожньою");

        var department = mapper.Map<Department>(createDto);
        await unitOfWork.Departments.AddAsync(department);
        await unitOfWork.SaveAsync();

        return mapper.Map<DepartmentDto>(department);
    }

    public async Task UpdateAsync(int id, UpdateDepartmentDto updateDto)
    {
        var department = await unitOfWork.Departments.GetByIdAsync(id);
        if (department == null)
            throw new EntityNotFoundException("Department", id);

        mapper.Map(updateDto, department);
        unitOfWork.Departments.Update(department);
        await unitOfWork.SaveAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var department = await unitOfWork.Departments.GetByIdAsync(id);
        if (department == null)
            return;

        unitOfWork.Departments.Remove(department);
        await unitOfWork.SaveAsync();
    }

    public async Task<List<SubjectDto>> GetSubjectsByDepartmentIdAsync(int departmentId)
    {
        var subjects = await unitOfWork.Subjects.GetByDepartmentIdAsync(departmentId);
        return mapper.Map<List<SubjectDto>>(subjects);
    }

    public async Task<List<TeacherDto>> GetTeachersByDepartmentIdAsync(int departmentId)
    {
        var teachers = await unitOfWork.Teachers.GetTeachersByDepartmentIdAsync(departmentId);
        return mapper.Map<List<TeacherDto>>(teachers);
    }

    public async Task<List<GroupDto>> GetGroupsByDepartmentIdAsync(int departmentId)
    {
        var groups = await unitOfWork.Groups.GetByDepartmentIdAsync(departmentId);
        return mapper.Map<List<GroupDto>>(groups);
    }
}
