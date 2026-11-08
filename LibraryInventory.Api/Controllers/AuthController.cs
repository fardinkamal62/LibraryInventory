using FluentValidation;
using LibraryInventory.Api.Dtos.Auth;
using LibraryInventory.Api.Extensions;
using LibraryInventory.Core.Commands;
using LibraryInventory.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryInventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService auth) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterDto dto, IValidator<RegisterDto> validator,
        CancellationToken ct)
    {
        var validation = await validator.ValidateAsync(dto, ct);
        if (!validation.IsValid) return this.ToValidationProblem(validation);

        var result = await auth.RegisterAsync(new RegisterCommand(dto.MemberName, dto.Email, dto.Password), ct);
        return result.IsSuccess ? Ok(ToDto(result.Data!)) : this.ToFailure(result.Error, result.Message);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginDto dto, IValidator<LoginDto> validator, CancellationToken ct)
    {
        var validation = await validator.ValidateAsync(dto, ct);
        if (!validation.IsValid) return this.ToValidationProblem(validation);

        var result = await auth.LoginAsync(new LoginCommand(dto.Email, dto.Password), ct);
        return result.IsSuccess ? Ok(ToDto(result.Data!)) : this.ToFailure(result.Error, result.Message);
    }

    private static AuthResponseDto ToDto(AuthResult result)
    {
        return new AuthResponseDto(result.Token, result.ExpiresAtUtc, result.MemberId, result.Role);
    }
}
