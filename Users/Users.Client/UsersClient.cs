using System.Net.Http.Json;
using Infrastructure;
using Infrastructure.Client;
using Users.Models;

namespace Users.Client;

public interface IUsersClient
{
    Task<Result<LoginUserResponseModel>> Register(LoginUserRequestModel request);
    Task<Result<LoginUserResponseModel>> Login(LoginUserRequestModel request);
}

public class UsersClient : IUsersClient
{
    private const string serviceName = "users"; // TODO: move service name in config/etc.
    private readonly IServiceDiscoveryClient sdClient;

    public UsersClient()
    {
        sdClient = new ServiceDiscoveryClient(serviceName, null); // TODO: config
    }

    public async Task<Result<LoginUserResponseModel>> Register(LoginUserRequestModel request)
    {
        var response = await sdClient.PostAsync("api/Users/Register", JsonContent.Create(request));
        if (!response.IsSuccess)
            return Result.ErrorFromHttp<LoginUserResponseModel>(response);

        var responseModel = await response.Value.Content.FromJsonOrThrow<LoginUserResponseModel>();
        return Result.Ok(responseModel);
    }

    public async Task<Result<LoginUserResponseModel>> Login(LoginUserRequestModel request)
    {
        var response = await sdClient.PostAsync("api/Users/Login", JsonContent.Create(request));
        if (!response.IsSuccess)
            return Result.ErrorFromHttp<LoginUserResponseModel>(response);

        var responseModel = await response.Value.Content.FromJsonOrThrow<LoginUserResponseModel>();
        return Result.Ok(responseModel);
    }
}