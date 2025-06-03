using ElectronicDepartment.BLL.Services.Interfaces;
using ElectronicDepartment.BLL.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ElectronicDepartment.PL.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubjectController(ISubjectService subjectService) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Отримати всі предмети", Description = "Повертає список усіх предметів у базі даних")]
    [SwaggerResponse(200, "Список предметів успішно отримано", typeof(IEnumerable<SubjectDto>))]
    public async Task<IActionResult> GetAll()
    {
        var disciplines = await subjectService.GetAllAsync();
        return Ok(disciplines);
    }

    [Authorize(Roles = "Admin,Manager,Teacher,Student")]
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Отримати предмет за ID", Description = "Повертає один предмет за його ідентифікатором")]
    [SwaggerResponse(200, "Предмет знайдено", typeof(SubjectDto))]
    [SwaggerResponse(404, "Предмет не знайдено")]
    public async Task<IActionResult> GetById(int id)
    {
        var discipline = await subjectService.GetByIdAsync(id);
        if (discipline == null)
            return NotFound();
        return Ok(discipline);
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost]
    [SwaggerOperation(Summary = "Створити новий предмет", Description = "Додає новий предмет до системи")]
    [SwaggerResponse(201, "Предмет створено", typeof(SubjectDto))]
    [SwaggerResponse(400, "Некоректні дані предмета")]
    public async Task<IActionResult> Create([FromBody] CreateSubjectDto createDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var discipline = await subjectService.AddAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = discipline.Id }, discipline);
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Оновити предмет", Description = "Оновлює дані предмета за ID")]
    [SwaggerResponse(204, "Предмет оновлено")]
    [SwaggerResponse(400, "ID не співпадає або дані некоректні")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSubjectDto updateDto)
    {
        if (!ModelState.IsValid || id != updateDto.Id)
            return BadRequest("ID не співпадає або дані некоректні");

        await subjectService.UpdateAsync(updateDto);
        return NoContent();
    }

    [Authorize(Roles = "Admin,Manager,Teacher,Student")]
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Видалити предмет", Description = "Видаляє предмет за ID")]
    [SwaggerResponse(204, "Предмет успішно видалено")]
    [SwaggerResponse(404, "Предмет не знайдено")]
    public async Task<IActionResult> Delete(int id)
    {
        await subjectService.DeleteAsync(id);
        return NoContent();
    }
}