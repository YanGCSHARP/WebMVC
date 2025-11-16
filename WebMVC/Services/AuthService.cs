using AutoMapper;
using WebMVC.Dtos;
using WebMVC.Models;
using WebMVC.Repositories.Interfaces;
using WebMVC.Services.Interfaces;

namespace WebMVC.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IMapper _mapper;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(IAuthRepository authRepository, IMapper mapper, IJwtTokenService jwtTokenService)
    {
        _authRepository = authRepository;
        _mapper = mapper;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<(UserDto userDto, string token)> RegisterAsync(RegisterDto registerDto)
    {
        if (await _authRepository.UserExists(registerDto.Email))
        {
            throw new IndexOutOfRangeException("Пользователь с таким email уже существует");
        }
        var user = _mapper.Map<User>(registerDto);
        var createdUser = await _authRepository.Register(user, registerDto.Password);
        var userDto = _mapper.Map<UserDto>(createdUser);
        var token = _jwtTokenService.GenerateToken(userDto);
        return (userDto, token);
    }

    public async Task<(UserDto? userDto, string? token)> LoginAsync(LoginDto loginDto)
    {
        var user = await _authRepository.Login(loginDto.Email, loginDto.Password);
        if (user == null)
            return (null, null);
        var userDto = _mapper.Map<UserDto>(user);
        var token = _jwtTokenService.GenerateToken(userDto);
        return (userDto, token);
    }
}