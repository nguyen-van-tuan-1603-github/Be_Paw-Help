using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawHelp.Data;
using PawHelp.DTOs.Auth;
using PawHelp.DTOs.Common;
using PawHelp.DTOs.User;
using PawHelp.Services;

namespace PawHelp.Controllers.Api;

[Authorize]
[Route("api/users")]
[ApiController]
public class UserApiController : ControllerBase
{
    private readonly PawHelpDbContext _context;
    private readonly FileUploadService _fileUploadService;

    public UserApiController(PawHelpDbContext context, FileUploadService fileUploadService)
    {
        _context = context;
        _fileUploadService = fileUploadService;
    }

    /// <summary>
    /// Lấy thông tin profile
    /// </summary>
    [HttpGet("profile")]
    public async Task<ActionResult<ApiResponse<UserInfo>>> GetProfile()
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

    /// <summary>
    /// Cập nhật profile
    /// </summary>
    [HttpPut("profile")]
    public async Task<ActionResult<ApiResponse<UserInfo>>> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse<UserInfo>.ErrorResponse("Dữ liệu không hợp lệ", errors));
        }

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

        // Cập nhật thông tin
        user.FullName = request.FullName;
        user.Phone = request.Phone;
        user.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();

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

        return Ok(ApiResponse<UserInfo>.SuccessResponse(userInfo, "Cập nhật thông tin thành công"));
    }

    /// <summary>
    /// Upload avatar
    /// </summary>
    [HttpPost("avatar")]
    public async Task<ActionResult<ApiResponse<string>>> UploadAvatar([FromForm] IFormFile avatar)
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(ApiResponse<string>.ErrorResponse("Token không hợp lệ"));
        }

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return NotFound(ApiResponse<string>.ErrorResponse("Không tìm thấy người dùng"));
        }

        // Upload ảnh
        var uploadResult = await _fileUploadService.UploadImageAsync(avatar, "uploads/avatars");
        if (!uploadResult.success)
        {
            return BadRequest(ApiResponse<string>.ErrorResponse(uploadResult.error ?? "Upload thất bại"));
        }

        // Xóa avatar cũ nếu có
        if (!string.IsNullOrEmpty(user.AvatarUrl))
        {
            _fileUploadService.DeleteFile(user.AvatarUrl);
        }

        // Cập nhật avatar URL
        user.AvatarUrl = uploadResult.filePath;
        user.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<string>.SuccessResponse(uploadResult.filePath!, "Upload avatar thành công"));
    }
}

