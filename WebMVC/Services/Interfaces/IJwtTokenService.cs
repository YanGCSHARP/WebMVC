using WebMVC.Dtos;
using WebMVC.Models;

namespace WebMVC.Services.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(UserDto userDto);
}