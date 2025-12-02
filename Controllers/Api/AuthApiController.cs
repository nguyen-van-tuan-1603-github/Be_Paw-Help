using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawHelp.Data;
using PawHelp.DTOs.Auth;
using PawHelp.DTOs.Common;
using PawHelp.Models.Entities;
using PawHelp.Services;
using System.Security.Claims;

namespace PawHelp.Controllers.Api;

[Route("api/auth")]
[ApiController]
public class AuthApiController : ControllerBase
{
    private readonly PawHelpDbContext _context;
    private readonly JwtService _jwtService;
    private readonly PasswordService _passwordService;

    public AuthApiController(PawHelpDbContext context, JwtService jwtService, PasswordService passwordService)
    {
        _context = context;
        _jwtService = jwtService;
        _passwordService = passwordService;
    }

    /// <summary>
    /// Đăng ký tài khoản mới
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse<AuthResponse>.ErrorResponse("Dữ liệu không hợp lệ", errors));
        }

        // Kiểm tra email đã tồn tại
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return BadRequest(ApiResponse<AuthResponse>.ErrorResponse("Email đã được sử dụng"));
        }

        // Kiểm tra số điện thoại đã tồn tại
        if (await _context.Users.AnyAsync(u => u.Phone == request.Phone))
        {
            return BadRequest(ApiResponse<AuthResponse>.ErrorResponse("Số điện thoại đã được sử dụng"));
        }

        // Tạo user mới
        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            Phone = request.Phone,
            PasswordHash = _passwordService.HashPassword(request.Password),
            UserRole = "user",
            Status = "active",
            EmailVerified = false,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Generate JWT token
        var token = _jwtService.GenerateToken(user);

        var response = new AuthResponse
        {
            Token = token,
            User = new UserInfo
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                AvatarUrl = user.AvatarUrl,
                UserRole = user.UserRole,
                Status = user.Status,
                EmailVerified = user.EmailVerified,
                CreatedAt = user.CreatedAt
            }
        };

        return Ok(ApiResponse<AuthResponse>.SuccessResponse(response, "Đăng ký thành công"));
    }

    /// <summary>
    /// Đăng nhập
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse<AuthResponse>.ErrorResponse("Dữ liệu không hợp lệ", errors));
        }

        // Tìm user theo email
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null)
        {
            return Unauthorized(ApiResponse<AuthResponse>.ErrorResponse("Email hoặc mật khẩu không đúng"));
        }

        // Kiểm tra mật khẩu
        if (!_passwordService.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Unauthorized(ApiResponse<AuthResponse>.ErrorResponse("Email hoặc mật khẩu không đúng"));
        }

        // Kiểm tra trạng thái tài khoản
        if (user.Status != "active")
        {
            return Unauthorized(ApiResponse<AuthResponse>.ErrorResponse("Tài khoản đã bị khóa"));
        }

        // Cập nhật last login
        user.LastLogin = DateTime.Now;
        await _context.SaveChangesAsync();

        // Generate JWT token
        var token = _jwtService.GenerateToken(user);

        var response = new AuthResponse
        {
            Token = token,
            User = new UserInfo
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                AvatarUrl = user.AvatarUrl,
                UserRole = user.UserRole,
                Status = user.Status,
                EmailVerified = user.EmailVerified,
                CreatedAt = user.CreatedAt
            }
        };

        return Ok(ApiResponse<AuthResponse>.SuccessResponse(response, "Đăng nhập thành công"));
    }

    /// <summary>
    /// Lấy thông tin user hiện tại (cần authentication)
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<ApiResponse<UserInfo>>> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(ApiResponse<UserInfo>.ErrorResponse("Token không hợp lệ"));
        }

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return NotFound(ApiResponse<UserInfo>.ErrorResponse("Không tìm thấy người dùng"));
        }

        var userInfo = new UserInfo
        {
            UserId = user.UserId,
            FullName = user.FullName,
            Email = user.Email,
            Phone = user.Phone,
            AvatarUrl = user.AvatarUrl,
            UserRole = user.UserRole,
            Status = user.Status,
            EmailVerified = user.EmailVerified,
            CreatedAt = user.CreatedAt
        };

        return Ok(ApiResponse<UserInfo>.SuccessResponse(userInfo));
    }
}

