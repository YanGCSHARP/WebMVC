using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using WebMVC.Dtos;
using WebMVC.Services.Interfaces;

namespace WebMVC.Controllers;

public class AccountController : Controller
{
    private readonly IAuthService _authService;

    public AccountController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [Route("Account/Register")]
    public async Task<IActionResult> RegisterForm([FromForm] RegisterDto registerDto)
    {
        if (!ModelState.IsValid)
            return View(registerDto);

        try
        {
            var (userDto, token) = await _authService.RegisterAsync(registerDto);
            await SignInUser(userDto, token);
            return RedirectToAction("Index", "Home");
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(registerDto);
        }
    }

    [HttpPost]
    [Route("Account/Register")]
    public async Task<IActionResult> RegisterJson([FromBody] RegisterDto registerDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var (userDto, token) = await _authService.RegisterAsync(registerDto);
            return Ok(new { user = userDto, token });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [Route("Account/Login")]
    public async Task<IActionResult> LoginForm([FromForm] LoginDto loginDto)
    {
        if (!ModelState.IsValid)
            return View(loginDto);

        var (userDto, token) = await _authService.LoginAsync(loginDto);

        if (userDto == null || token == null)
        {
            ModelState.AddModelError(string.Empty, "Неверный email или пароль");
            return View(loginDto);
        }

        await SignInUser(userDto, token);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Route("Account/Login")]
    public async Task<IActionResult> LoginJson([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (userDto, token) = await _authService.LoginAsync(loginDto);

        if (userDto == null || token == null)
            return Unauthorized(new { message = "Неверный email или пароль" });

        return Ok(new { user = userDto, token });
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        Response.Cookies.Delete("jwt");
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    private async Task SignInUser(UserDto userDto, string token)
    {
        Response.Cookies.Append("jwt", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddHours(24)
        });

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userDto.Id.ToString()),
            new Claim(ClaimTypes.Name, userDto.Username),
            new Claim(ClaimTypes.Email, userDto.Email)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);
    }
}
