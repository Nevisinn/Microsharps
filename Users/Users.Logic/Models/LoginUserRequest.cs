namespace Users.Logic.Models;

public class LoginUserRequest
{
    public required string Login { get; set; }
    public required string Password { get; set; }
}