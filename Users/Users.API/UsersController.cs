using Infrastructure.API;
using Microsoft.AspNetCore.Mvc;
using Users.Logic;
using Users.Models;

namespace Users.API;

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
        var response = await usersService.RegisterUser(Mapper.Map(request));
        return response.ActionResult(Mapper.Map);
    }
    
    /// <summary>
    /// Вход юзера
    /// </summary>
    [HttpPost("Login")]
    public async Task<ActionResult<LoginUserResponseModel>> Login(LoginUserRequestModel request)
    {
        var response = await usersService.LoginUser(Mapper.Map(request));
        return response.ActionResult(Mapper.Map);
    }
}