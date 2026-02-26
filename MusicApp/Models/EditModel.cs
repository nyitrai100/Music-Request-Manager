namespace MusicApp.Models;

public class EditModel
{
    public required string Id { get; set; }
    public required string Email { get; set; }
    public required string Username { get; set; }
    public required string Role { get; set; }

    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }

    public string? EditError { get; set; }
}