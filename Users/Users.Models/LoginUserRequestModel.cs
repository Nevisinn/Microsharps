namespace Users.Models;

public class LoginUserRequestModel
{
    public required string Login { get; set; }
    public required string Password { get; set; }
}