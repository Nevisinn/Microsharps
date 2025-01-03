using Infrastructure;
using Users.Client;
using Users.Models;

namespace ApiGateway.Logic.Users;

public interface IUsersService
{
    Task<Result<LoginUserResponseModel>> Register(LoginUserRequestModel requestModel);
    Task<Result<LoginUserResponseModel>> Login(LoginUserRequestModel requestModel);
}

public class UsersService : IUsersService
{
    private readonly IUsersClient client;

    public UsersService(IUsersClient client)
    {
        this.client = client;
    }

    public async Task<Result<LoginUserResponseModel>> Register(LoginUserRequestModel requestModel)
    {
        return await client.Register(requestModel);
    }

    public async Task<Result<LoginUserResponseModel>> Login(LoginUserRequestModel requestModel)
    {
        return await client.Login(requestModel);
    }
}