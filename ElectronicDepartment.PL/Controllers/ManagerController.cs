using ElectronicDepartment.BLL.DTOs;
using ElectronicDepartment.BLL.Services.Interfaces;
using ElectronicDepartment.Common.Exceptions;
using ElectronicDepartment.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ElectronicDepartment.PL.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class ManagerController(IManagerService managerService) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Отримати всіх менеджерів", Description = "Повертає список усіх менеджерів у базі даних")]
    [SwaggerResponse(200, "Список менеджерів успішно отримано", typeof(List<Manager>))]
    public async Task<ActionResult<List<Manager>>> GetAll() =>
        Ok(await managerService.GetAllAsync());

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Отримати менеджера за ID", Description = "Повертає одного менеджера за його ідентифікатором")]
    [SwaggerResponse(200, "Менеджера знайдено", typeof(Manager))]
    [SwaggerResponse(404, "Менеджера не знайдено")]
    public async Task<ActionResult<Manager>> GetById(int id)
    {
        var manager = await managerService.GetByIdAsync(id);
        if (manager == null) return NotFound();
        return Ok(manager);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Створити нового менеджера", Description = "Додає нового менеджера до системи")]
    [SwaggerResponse(201, "Менеджера створено", typeof(Manager))]
    [SwaggerResponse(400, "Некоректні дані менеджера")]
    public async Task<ActionResult> Create([FromBody] CreateManagerDto createDto)
    {
        var manager = await managerService.AddAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = manager.Id }, manager);
    }

    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Оновити дані менеджера", Description = "Оновлює інформацію про менеджера за ID")]
    [SwaggerResponse(204, "Менеджера оновлено")]
    [SwaggerResponse(400, "ID не збігається або дані некоректні")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateManagerDto updateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await managerService.UpdateAsync(id, updateDto);
            return NoContent();
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Видалити менеджера", Description = "Видаляє менеджера за вказаним ID")]
    [SwaggerResponse(204, "Менеджера успішно видалено")]
    public async Task<ActionResult> Delete(int id)
    {
        await managerService.DeleteAsync(id);
        return NoContent();
    }
}
