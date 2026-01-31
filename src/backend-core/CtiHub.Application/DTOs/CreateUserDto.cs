namespace CtiHub.Application.DTOs;

public class CreateUserDto
{
    public string Username { get; set; } = string.Empty; // Varsayılan boş
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}