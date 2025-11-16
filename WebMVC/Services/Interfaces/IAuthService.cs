using WebMVC.Dtos;

namespace WebMVC.Services.Interfaces;

public interface IAuthService
{
    Task<(UserDto userDto, string token)> RegisterAsync(RegisterDto registerDto);
    Task<(UserDto? userDto, string? token)> LoginAsync(LoginDto loginDto);
}