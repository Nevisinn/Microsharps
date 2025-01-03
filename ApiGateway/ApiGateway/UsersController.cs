using System.Security.Claims;
using ApiGateway.Logic.Users;
using Infrastructure.API;
using Infrastructure.API.Configuration.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Users.Models;

namespace ApiGateway;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUsersService usersService;

    public UsersController(IUsersService usersService)
    {
        this.usersService = usersService;
    }

    /// <summary>
    /// Регистрация юзера
    /// </summary>
    [HttpPost("Register")]
    public async Task<ActionResult<LoginUserResponseModel>> Register(LoginUserRequestModel request)
    {
        var response = await usersService.Register(request);
        if (response.IsSuccess)
            await SetAuthCookie(response.Value.UserId);
        return response.ActionResult();
    }
    
    /// <summary>
    /// Вход юзера
    /// </summary>
    [HttpPost("Login")]
    public async Task<ActionResult<LoginUserResponseModel>> Login(LoginUserRequestModel request)
    {
        var response = await usersService.Login(request);
        if (response.IsSuccess)
            await SetAuthCookie(response.Value.UserId);
        return response.ActionResult();
    }

    /// <summary>
    /// Сброс сессии
    /// </summary>
    [Authorize]
    [HttpPost("Logout")]
    public async Task<ActionResult> Logout()
    {
        await HttpContext.SignOutAsync(AuthConfiguration.Scheme);
        return NoContent();
    }
    
    /// <summary>
    /// Инфа о сессии
    /// </summary>
    [Authorize]
    [HttpPost("Info")]
    public async Task<ActionResult<Guid>> Info()
    {
        return Ok(new { UserId = User.GetId() });
    }


    private async Task SetAuthCookie(Guid userId)
    {
        var credentials = GetCredentials(userId);
        await HttpContext.SignInAsync(AuthConfiguration.Scheme, new ClaimsPrincipal(credentials));
    }
    
    private ClaimsIdentity GetCredentials(Guid userId)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        };
        return new ClaimsIdentity(claims, AuthConfiguration.Scheme);
    }
}