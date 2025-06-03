using ElectronicDepartment.BLL.DTOs;
using ElectronicDepartment.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ElectronicDepartment.PL.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    [SwaggerOperation(
        Summary = "Авторизація користувача",
        Description = "Авторизує користувача за email та паролем і повертає JWT токен"
    )]
    [SwaggerResponse(200, "Успішна авторизація", typeof(AuthResponseDto))]
    [SwaggerResponse(401, "Невірні дані для входу")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var response = await authService.LoginAsync(loginDto.Email, loginDto.Password);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
}
