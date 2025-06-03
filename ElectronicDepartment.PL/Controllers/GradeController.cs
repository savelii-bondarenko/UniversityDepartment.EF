using ElectronicDepartment.BLL.DTOs;
using ElectronicDepartment.BLL.Services.Interfaces;
using ElectronicDepartment.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ElectronicDepartment.PL.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GradeController(IGradeService gradeService) : ControllerBase
{
    [Authorize(Roles = "Student,Manager,Admin,Teacher")]
    [HttpGet]
    [SwaggerOperation(Summary = "Отримати всі оцінки", Description = "Повертає список усіх оцінок або фільтрований список")]
    [SwaggerResponse(200, "Список оцінок", typeof(List<GradeDto>))]
    public async Task<IActionResult> GetAll([FromQuery] GradeFilterDto? filter = null)
    {
        var grades = filter != null
            ? await gradeService.GetFilteredAsync(filter)
            : await gradeService.GetAllAsync();
        return Ok(grades);
    }

    [Authorize(Roles = "Teacher")]
    [HttpPost]
    [SwaggerOperation(Summary = "Додати оцінку", Description = "Викладач додає оцінку студенту")]
    [SwaggerResponse(201, "Оцінку створено", typeof(GradeDto))]
    [SwaggerResponse(400, "Некоректні дані")]
    [SwaggerResponse(403, "Немає прав для додавання оцінки")]
    public async Task<IActionResult> Create([FromBody] CreateGradeDto createDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var grade = await gradeService.AddGradeAsync(createDto);
            return CreatedAtAction(nameof(GetAll), grade);
        }
        catch (Common.Exceptions.UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = "Teacher")]
    [HttpDelete("{studentId}/{subjectId}")]
    [SwaggerOperation(Summary = "Видалити оцінку", Description = "Викладач видаляє оцінку студента")]
    [SwaggerResponse(204, "Оцінку видалено")]
    [SwaggerResponse(403, "Немає прав для видалення оцінки")]
    public async Task<IActionResult> Delete(int studentId, int subjectId, [FromQuery] int teacherId)
    {
        try
        {
            await gradeService.DeleteGradeAsync(studentId, subjectId, teacherId);
            return NoContent();
        }
        catch (Common.Exceptions.UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}
