using ElectronicDepartment.BLL.DTOs;
using ElectronicDepartment.BLL.Services.Interfaces;
using ElectronicDepartment.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;


namespace ElectronicDepartment.PL.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GroupController(IGroupService groupService) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    [SwaggerOperation(Summary = "Створити групу", Description = "Створює нову групу. Доступно для адміністраторів і менеджерів.")]
    [SwaggerResponse(201, "Групу створено", typeof(GroupDto))]
    [SwaggerResponse(400, "Некоректні дані")]
    [SwaggerResponse(404, "Кафедра не знайдена")]
    [SwaggerResponse(500, "Внутрішня помилка сервера")]
    public async Task<IActionResult> Create([FromBody] CreateGroupDto createDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var group = await groupService.AddAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = group.Id }, group);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { ex.Message });
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(new { ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { Message = "Виникла помилка при створенні групи." });
        }
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,Manager,Teacher,Student")]
    [SwaggerOperation(Summary = "Отримати групу за ID", Description = "Повертає групу за її ID.")]
    [SwaggerResponse(200, "Групу знайдено", typeof(GroupDto))]
    [SwaggerResponse(404, "Групу не знайдено")]
    [SwaggerResponse(500, "Внутрішня помилка сервера")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var group = await groupService.GetByIdAsync(id);
            if (group == null)
            {
                return NotFound(new { Message = $"Групу з ID {id} не знайдено." });
            }

            return Ok(group);
        }
        catch (Exception)
        {
            return StatusCode(500, new { Message = "Виникла помилка при отриманні групи." });
        }
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager,Teacher,Student")]
    [SwaggerOperation(Summary = "Отримати всі групи", Description = "Повертає список усіх груп без фільтрації.")]
    [SwaggerResponse(200, "Список груп", typeof(List<GroupDto>))]
    [SwaggerResponse(500, "Внутрішня помилка сервера")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var groups = await groupService.GetAllAsync();
            return Ok(groups);
        }
        catch (Exception)
        {
            return StatusCode(500, new { Message = "Виникла помилка при отриманні груп." });
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    [SwaggerOperation(Summary = "Оновити групу", Description = "Оновлює дані групи за ID. Доступно для адміністраторів і менеджерів.")]
    [SwaggerResponse(204, "Групу успішно оновлено")]
    [SwaggerResponse(400, "Некоректні дані")]
    [SwaggerResponse(404, "Групу або кафедру не знайдено")]
    [SwaggerResponse(500, "Внутрішня помилка сервера")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateGroupDto updateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await groupService.UpdateAsync(id, updateDto);
            return NoContent();
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { ex.Message });
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(new { ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { Message = "Виникла помилка при оновленні групи." });
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    [SwaggerOperation(Summary = "Видалити групу", Description = "Видаляє групу за ID. Доступно для адміністраторів і менеджерів.")]
    [SwaggerResponse(204, "Групу успішно видалено")]
    [SwaggerResponse(400, "Неможливо видалити групу через пов’язані записи")]
    [SwaggerResponse(404, "Групу не знайдено")]
    [SwaggerResponse(500, "Внутрішня помилка сервера")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await groupService.DeleteAsync(id);
            return NoContent();
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { Message = "Виникла помилка при видаленні групи." });
        }
    }
}
