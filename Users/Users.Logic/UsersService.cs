using System.Security.Claims;
using Infrastructure;
using Users.DAL;
using Users.Logic.Models;

namespace Users.Logic;

public interface IUsersService
{
    Task<Result<LoginUserResponse>> RegisterUser(LoginUserRequest request);
    Task<Result<LoginUserResponse>> LoginUser(LoginUserRequest request);
}

public class UsersService : IUsersService
{
    private readonly IUsersRepository repository;

    public UsersService(IUsersRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Result<LoginUserResponse>> RegisterUser(LoginUserRequest request)
    {
        var existed = await repository.GetUser(request.Login);
        if (existed != null)
            return Result.BadRequest<LoginUserResponse>("Login is already taken");

        var added = await repository.RegisterUser(new UserEntity()
        {
            Login = request.Login,
            Password = request.Password,
        });

        return Result.Ok(new LoginUserResponse()
        {
            UserId = added.Id,
        });
    }

    public async Task<Result<LoginUserResponse>> LoginUser(LoginUserRequest request)
    {
        var existed = await repository.GetUser(request.Login);
        if (existed == null || existed.Password != request.Password)
            return Result.BadRequest<LoginUserResponse>("Login or password is incorrect");

        return Result.Ok(new LoginUserResponse()
        {
            UserId = existed.Id,
        });
    }
}