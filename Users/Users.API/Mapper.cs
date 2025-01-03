using Users.Logic.Models;
using Users.Models;

namespace Users.API;

public static class Mapper
{
    public static LoginUserRequest Map(LoginUserRequestModel model)
        => new()
        {
            Login = model.Login,
            Password = model.Password,
        };
    
    public static LoginUserResponseModel Map(LoginUserResponse response)
        => new()
        {
            UserId = response.UserId
        };
}