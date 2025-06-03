using ElectronicDepartment.BLL.DTOs;
using ElectronicDepartment.BLL.Services.Interfaces;
using ElectronicDepartment.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ElectronicDepartment.PL.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentController : ControllerBase
{
    private readonly IDepartmentService _departmentService;

    public DepartmentController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Отримати всі кафедри", Description = "Повертає список усіх кафедр")]
    [SwaggerResponse(200, "Список кафедр", typeof(List<DepartmentDto>))]
    public async Task<IActionResult> GetAll()
    {
        var departments = await _departmentService.GetAllAsync();
        return Ok(departments);
    }

    [Authorize(Roles = "Student,Manager,Admin,Teacher")]
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Отримати кафедру за ID", Description = "Повертає кафедру за її ідентифікатором")]
    [SwaggerResponse(200, "Кафедру знайдено", typeof(DepartmentDto))]
    [SwaggerResponse(404, "Кафедру не знайдено")]
    public async Task<IActionResult> GetById(int id)
    {
        var department = await _departmentService.GetByIdAsync(id);
        if (department == null)
            return NotFound();
        return Ok(department);
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost]
    [SwaggerOperation(Summary = "Створити нову кафедру", Description = "Додає нову кафедру до системи")]
    [SwaggerResponse(201, "Кафедру створено", typeof(DepartmentDto))]
    [SwaggerResponse(400, "Некоректні дані")]
    public async Task<IActionResult> Create([FromBody] CreateDepartmentDto createDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var department = await _departmentService.AddAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = department.Id }, department);
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Оновити кафедру", Description = "Оновлює дані кафедри")]
    [SwaggerResponse(204, "Кафедру оновлено")]
    [SwaggerResponse(400, "Некоректні дані")]
    [SwaggerResponse(404, "Кафедру не знайдено")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDepartmentDto updateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _departmentService.UpdateAsync(id, updateDto);
            return NoContent();
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Видалити кафедру", Description = "Видаляє кафедру за ID")]
    [SwaggerResponse(204, "Кафедру видалено")]
    public async Task<IActionResult> Delete(int id)
    {
        await _departmentService.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("{id}/subjects")]
    [SwaggerOperation(Summary = "Отримати предмети кафедри", Description = "Повертає список предметів для кафедри за ID")]
    [SwaggerResponse(200, "Предмети отримано", typeof(List<SubjectDto>))]
    [SwaggerResponse(404, "Кафедру не знайдено")]
    public async Task<IActionResult> GetSubjects(int id)
    {
        try
        {
            var subjects = await _departmentService.GetSubjectsByDepartmentIdAsync(id);
            return Ok(subjects);
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("{id}/teachers")]
    [SwaggerOperation(Summary = "Отримати викладачів кафедри", Description = "Повертає список викладачів для кафедри за ID")]
    [SwaggerResponse(200, "Викладачів отримано", typeof(List<TeacherDto>))]
    [SwaggerResponse(404, "Кафедру не знайдено")]
    public async Task<IActionResult> GetTeachers(int id)
    {
        try
        {
            var teachers = await _departmentService.GetTeachersByDepartmentIdAsync(id);
            return Ok(teachers);
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("{id}/groups")]
    [SwaggerOperation(Summary = "Отримати групи кафедри", Description = "Повертає список груп для кафедри за ID")]
    [SwaggerResponse(200, "Групи отримано", typeof(List<GroupDto>))]
    [SwaggerResponse(404, "Кафедру не знайдено")]
    public async Task<IActionResult> GetGroups(int id)
    {
        try
        {
            var groups = await _departmentService.GetGroupsByDepartmentIdAsync(id);
            return Ok(groups);
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }
    }
}
