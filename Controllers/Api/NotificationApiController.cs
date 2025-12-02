using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawHelp.Data;
using PawHelp.DTOs.Common;

namespace PawHelp.Controllers.Api;

[Authorize]
[Route("api/notifications")]
[ApiController]
public class NotificationApiController : ControllerBase
{
    private readonly PawHelpDbContext _context;

    public NotificationApiController(PawHelpDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lấy danh sách thông báo của user
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<NotificationDto>>>> GetNotifications()
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(ApiResponse<List<NotificationDto>>.ErrorResponse("Token không hợp lệ"));
        }

        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(50) // Giới hạn 50 thông báo gần nhất
            .Select(n => new NotificationDto
            {
                NotificationId = n.NotificationId,
                Title = n.Title,
                Message = n.Message,
                Type = n.Type,
                RelatedPostId = n.RelatedPostId,
                Icon = n.Icon,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            })
            .ToListAsync();

        return Ok(ApiResponse<List<NotificationDto>>.SuccessResponse(notifications));
    }

    /// <summary>
    /// Đánh dấu thông báo đã đọc
    /// </summary>
    [HttpPut("{id}/read")]
    public async Task<ActionResult<ApiResponse<object>>> MarkAsRead(int id)
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(ApiResponse<object>.ErrorResponse("Token không hợp lệ"));
        }

        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.NotificationId == id && n.UserId == userId);

        if (notification == null)
        {
            return NotFound(ApiResponse<object>.ErrorResponse("Không tìm thấy thông báo"));
        }

        notification.IsRead = true;
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<object>.SuccessResponse(null, "Đánh dấu đã đọc thành công"));
    }

    /// <summary>
    /// Đánh dấu tất cả thông báo đã đọc
    /// </summary>
    [HttpPut("read-all")]
    public async Task<ActionResult<ApiResponse<object>>> MarkAllAsRead()
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(ApiResponse<object>.ErrorResponse("Token không hợp lệ"));
        }

        await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ExecuteUpdateAsync(n => n.SetProperty(x => x.IsRead, true));

        return Ok(ApiResponse<object>.SuccessResponse(null, "Đã đánh dấu tất cả đã đọc"));
    }
}

public class NotificationDto
{
    public int NotificationId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int? RelatedPostId { get; set; }
    public string? Icon { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}

