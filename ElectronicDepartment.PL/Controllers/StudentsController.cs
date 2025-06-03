using ElectronicDepartment.BLL.DTOs;
using ElectronicDepartment.BLL.Services.Interfaces;
using ElectronicDepartment.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ElectronicDepartment.PL.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController(IStudentService studentService) : ControllerBase
{
    [Authorize(Roles = "Manager,Admin,Teacher")]
    [HttpGet]
    [SwaggerOperation(Summary = "Отримати всіх студентів", Description = "Повертає список усіх студентів або фільтрований список")]
    [SwaggerResponse(200, "Успішно отримано список студентів", typeof(List<StudentDto>))]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var students = await studentService.GetAllAsync();
            return Ok(students);
        }
        catch (Exception)
        {
            return StatusCode(500, new { Message = "Виникла помилка при отриманні студентів." });
        }
    }

    [Authorize(Roles = "Manager,Admin,Teacher")]
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Отримати студента за ID", Description = "Повертає одного студента за вказаним ID")]
    [SwaggerResponse(200, "Студента знайдено", typeof(StudentDto))]
    [SwaggerResponse(404, "Студента не знайдено")]
    public async Task<IActionResult> GetById(int id)
    {
        var student = await studentService.GetByIdAsync(id);
        if (student == null)
            return NotFound();
        return Ok(student);
    }

    [Authorize(Roles = "Manager,Admin")]
    [HttpPost]
    [SwaggerOperation(Summary = "Створити нового студента", Description = "Додає нового студента до системи")]
    [SwaggerResponse(201, "Студента створено", typeof(StudentDto))]
    [SwaggerResponse(400, "Некоректні дані студента")]
    public async Task<IActionResult> Create([FromBody] CreateStudentDto createDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var student = await studentService.AddAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = student.Id }, student);
    }

    [Authorize(Roles = "Manager,Admin")]
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Оновити дані студента", Description = "Оновлює дані студента за ID")]
    [SwaggerResponse(204, "Студента оновлено")]
    [SwaggerResponse(400, "Некоректні дані")]
    [SwaggerResponse(404, "Студента не знайдено")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateStudentDto updateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await studentService.UpdateAsync(id, updateDto);
            return NoContent();
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }
    }

    [Authorize(Roles = "Manager,Admin")]
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Видалити студента", Description = "Видаляє студента з бази за вказаним ID")]
    [SwaggerResponse(204, "Студента видалено")]
    public async Task<IActionResult> Delete(int id)
    {
        await studentService.DeleteAsync(id);
        return NoContent();
    }

    [Authorize(Roles = "Manager,Admin,Teacher")]
    [HttpGet("{id}/grades")]
    [SwaggerOperation(Summary = "Отримати оцінки студента", Description = "Повертає список оцінок студента за ID")]
    [SwaggerResponse(200, "Оцінки студента отримано", typeof(List<GradeDto>))]
    [SwaggerResponse(404, "Студента не знайдено")]
    public async Task<IActionResult> GetGrades(int id)
    {
        var student = await studentService.GetByIdAsync(id);
        if (student == null)
            return NotFound();

        var grades = await studentService.GetGradesByStudentIdAsync(id);
        return Ok(grades);
    }

    [Authorize(Roles = "Manager,Admin,Teacher")]
    [HttpGet("{id}/subjects")]
    [SwaggerOperation(Summary = "Отримати предмети студента", Description = "Повертає список предметів студента за ID")]
    [SwaggerResponse(200, "Предмети студента отримано", typeof(List<SubjectDto>))]
    [SwaggerResponse(404, "Студента не знайдено")]
    public async Task<IActionResult> GetSubjects(int id)
    {
        var student = await studentService.GetByIdAsync(id);
        if (student == null)
            return NotFound();

        var subjects = await studentService.GetSubjectsByStudentIdAsync(id);
        return Ok(subjects);
    }
}
