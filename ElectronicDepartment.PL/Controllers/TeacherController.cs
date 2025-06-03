using ElectronicDepartment.BLL.DTOs;
using ElectronicDepartment.BLL.Services.Interfaces;
using ElectronicDepartment.Common.Exceptions;
using ElectronicDepartment.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ElectronicDepartment.PL.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeacherController(ITeacherService teacherService) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Отримати список усіх викладачів", Description = "Повертає всі записи викладачів із бази даних")]
    [SwaggerResponse(200, "Список викладачів успішно отримано", typeof(IEnumerable<Teacher>))]
    public async Task<IActionResult> GetAll()
    {
        var teachers = await teacherService.GetAllAsync();
        return Ok(teachers);
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Отримати викладача за ID", Description = "Повертає одного викладача за заданим ID")]
    [SwaggerResponse(200, "Викладача знайдено", typeof(Teacher))]
    [SwaggerResponse(404, "Викладача не знайдено")]
    public async Task<IActionResult> GetById(int id)
    {
        var teacher = await teacherService.GetByIdAsync(id);
        if (teacher is null)
            return NotFound();

        return Ok(teacher);
    }

    [HttpGet("email/{email}")]
    [Authorize(Roles = "Manager,Admin")]
    [SwaggerOperation(Summary = "Отримати викладача за Email", Description = "Пошук викладача за електронною адресою")]
    [SwaggerResponse(200, "Викладача знайдено", typeof(Teacher))]
    [SwaggerResponse(404, "Викладача не знайдено")]
    public async Task<IActionResult> GetByEmail(string email)
    {
        var teacher = await teacherService.GetByEmailAsync(email);
        if (teacher is null)
            return NotFound();

        return Ok(teacher);
    }

    [HttpPost]
    [Authorize(Roles = "Manager,Admin")]
    [SwaggerOperation(Summary = "Створити нового викладача", Description = "Додає нового викладача до бази")]
    [SwaggerResponse(201, "Викладача створено", typeof(Teacher))]
    [SwaggerResponse(400, "Невірна модель даних")]
    public async Task<IActionResult> Create([FromBody] CreateTeacherDto createDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var teacher = await teacherService.AddAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = teacher.Id }, teacher);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Manager,Admin")]
    [SwaggerOperation(Summary = "Оновити викладача", Description = "Оновлює дані викладача за ID. Доступно лише для менеджерів і адміністраторів.")]
    [SwaggerResponse(204, "Викладача успішно оновлено")]
    [SwaggerResponse(400, "Некоректні дані або ID не співпадає")]
    [SwaggerResponse(404, "Викладача не знайдено")]
    [SwaggerResponse(500, "Внутрішня помилка сервера")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTeacherDto updateDto)
    {
        if (updateDto == null)
            return BadRequest(new { Message = "Дані для оновлення не можуть бути порожніми." });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await teacherService.UpdateAsync(id, updateDto);
            return NoContent();
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(new { ex.Message });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { Message = "Виникла помилка при оновленні викладача." });
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Manager,Admin")]
    [SwaggerOperation(Summary = "Видалити викладача", Description = "Видаляє викладача за ID")]
    [SwaggerResponse(204, "Викладача успішно видалено")]
    public async Task<IActionResult> Delete(int id)
    {
        await teacherService.DeleteAsync(id);
        return NoContent();
    }
}