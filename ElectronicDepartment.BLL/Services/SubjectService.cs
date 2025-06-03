using ElectronicDepartment.BLL.DTOs;
using ElectronicDepartment.BLL.Services.Interfaces;
using ElectronicDepartment.Common.Exceptions;
using ElectronicDepartment.DAL.Entities;
using ElectronicDepartment.DAL.UnitOfWork.Interfaces;
using AutoMapper;

namespace ElectronicDepartment.BLL.Services;

public class SubjectService(IUnitOfWork unitOfWork, IMapper mapper) : ISubjectService
{
    public async Task<List<SubjectDto>> GetAllAsync()
    {
        var subjects = await unitOfWork.Subjects.GetAllAsync();
        return mapper.Map<List<SubjectDto>>(subjects);
    }

    public async Task<SubjectDto?> GetByIdAsync(int id)
    {
        var subject = await unitOfWork.Subjects.GetByIdAsync(id);
        return subject == null ? null : mapper.Map<SubjectDto>(subject);
    }

    public async Task<SubjectDto> AddAsync(CreateSubjectDto createDto)
    {
        if (string.IsNullOrEmpty(createDto.Name))
            throw new ValidationException("Назва предмета не може бути порожньою");

        var teacher = await unitOfWork.Teachers.GetByIdAsync(createDto.TeacherId);
        if (teacher == null)
            throw new EntityNotFoundException("Teacher", createDto.TeacherId);

        var subject = mapper.Map<Subject>(createDto);
        await unitOfWork.Subjects.AddAsync(subject);
        await unitOfWork.SaveAsync();

        return mapper.Map<SubjectDto>(subject);
    }

    public async Task UpdateAsync(UpdateSubjectDto updateDto)
    {
        var subject = await unitOfWork.Subjects.GetByIdAsync(updateDto.Id);
        if (subject == null)
            throw new EntityNotFoundException("Subject", updateDto.Id);

        var teacher = await unitOfWork.Teachers.GetByIdAsync(updateDto.TeacherId);
        if (teacher == null)
            throw new EntityNotFoundException("Teacher", updateDto.TeacherId);

        mapper.Map(updateDto, subject);
        unitOfWork.Subjects.Update(subject);
        await unitOfWork.SaveAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var subject = await unitOfWork.Subjects.GetByIdAsync(id);
        if (subject == null)
            return;

        unitOfWork.Subjects.Remove(subject);
        await unitOfWork.SaveAsync();
    }
}