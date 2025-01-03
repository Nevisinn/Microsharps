using System.ComponentModel.DataAnnotations;

namespace Users.DAL;

public class UserEntity
{
    [Key]
    public Guid Id { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
}