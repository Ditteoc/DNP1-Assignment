namespace DTOs;

public class UpdateUserDTO
{
    public required int Id { get; set; }
    public required string Username { get; set; }
    public required string? Password { get; set; }
}