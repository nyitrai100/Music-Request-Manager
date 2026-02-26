
namespace MusicApp.Models;

public class RegisterModel
{
    public required string Email { get; set; }
    public required string Role { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string ConfirmPassword { get; set; }
    public string? RegisterError { get; set; }
}