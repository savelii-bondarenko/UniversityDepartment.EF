using ElectronicDepartment.BLL.DTOs;
using ElectronicDepartment.BLL.Services.Interfaces;
using ElectronicDepartment.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ElectronicDepartment.PL.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Отримати всіх адміністраторів", Description = "Повертає список усіх адміністраторів із бази даних")]
    [SwaggerResponse(200, "Список адміністраторів успішно отримано", typeof(List<AdminDto>))]
    public async Task<ActionResult<List<AdminDto>>> GetAll()
    {
        var admins = await _adminService.GetAllAsync();
        return Ok(admins);
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Отримати адміністратора за ID",Description = "Повертає дані адміністратора за його унікальним ідентифікатором")]
    [SwaggerResponse(200, "Адміністратора знайдено", typeof(AdminDto))]
    [SwaggerResponse(404, "Адміністратора не знайдено")]
    public async Task<ActionResult<AdminDto>> GetById(int id)
    {
        var admin = await _adminService.GetByIdAsync(id);
        if (admin == null)
            return NotFound();
        return Ok(admin);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Створити нового адміністратора",Description = "Додає нового адміністратора до системи")]
    [SwaggerResponse(201, "Адміністратора створено", typeof(AdminDto))]
    [SwaggerResponse(400, "Некоректні дані адміністратора")]
    public async Task<ActionResult<AdminDto>> Create([FromBody] CreateAdminDto createDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var admin = await _adminService.AddAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = admin.Id }, admin);
    }

    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Оновити адміністратора", Description = "Оновлює інформацію про адміністратора за вказаним ID")]
    [SwaggerResponse(204, "Адміністратора оновлено")]
    [SwaggerResponse(400, "Некоректні дані")]
    [SwaggerResponse(404, "Адміністратора не знайдено")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateAdminDto updateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _adminService.UpdateAsync(id, updateDto);
            return NoContent();
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Видалити адміністратора",Description = "Видаляє адміністратора з бази даних за вказаним ID")]
    [SwaggerResponse(204, "Адміністратора успішно видалено")]
    public async Task<ActionResult> Delete(int id)
    {
        await _adminService.DeleteAsync(id);
        return NoContent();
    }
}
