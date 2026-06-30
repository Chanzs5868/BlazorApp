using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using BlazorApp.Models;
using BlazorApp.Services;

namespace BlazorApp.Controllers;

[ApiController]
[Route("api/[controller]")]
[IgnoreAntiforgeryToken]
public class AuthController : ControllerBase
{
    private readonly UserService _userService;
    private readonly JwtService _jwtService;

    public AuthController(UserService userService, JwtService jwtService)
    {
        _userService = userService;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public IActionResult Register(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { message = "Username and password are required." });

        if (_userService.GetByUsername(request.Username) is not null)
            return BadRequest(new { message = "Username already taken." });

        var user = _userService.Register(request.Username, request.Password);
        var token = _jwtService.GenerateToken(user);

        return Ok(new AuthResponse { Token = token, Username = user.Username, IsAdmin = user.IsAdmin });
    }

    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        var user = _userService.GetByUsername(request.Username);
        if (user is null || !_userService.VerifyPassword(user, request.Password))
            return Unauthorized(new { message = "Invalid username or password." });

        var token = _jwtService.GenerateToken(user);
        return Ok(new AuthResponse { Token = token, Username = user.Username, IsAdmin = user.IsAdmin });
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = _userService.GetById(userId);
        if (user is null) return NotFound();

        return Ok(new ProfileResponse { Username = user.Username, IsAdmin = user.IsAdmin });
    }

    [HttpPost("change-password")]
    [Authorize]
    public IActionResult ChangePassword(ChangePasswordRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        if (string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length < 4)
            return BadRequest(new { message = "New password must be at least 4 characters." });

        var success = _userService.ChangePassword(userId, request.OldPassword, request.NewPassword);
        if (!success)
            return BadRequest(new { message = "Old password is incorrect." });

        return Ok(new { message = "Password changed successfully." });
    }
    
    public class SetAdminRequest
    {
        public string Username { get; set; } = "";
        public bool IsAdmin { get; set; } = true;
    }

    [HttpPost("set-admin")]
    [Authorize]
    public IActionResult SetAdmin(SetAdminRequest request)
    {
        var isAdmin = User.Claims.FirstOrDefault(c => c.Type == "IsAdmin")?.Value == "True";
        if (!isAdmin) return Forbid();

        var success = _userService.SetAdmin(request.Username, request.IsAdmin);
        return success ? Ok(new { message = $"{request.Username} admin status set to {request.IsAdmin}." }) : NotFound();
    }
}