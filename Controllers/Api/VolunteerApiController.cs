using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawHelp.Data;
using PawHelp.DTOs.Common;
using PawHelp.Models.Entities;

namespace PawHelp.Controllers.Api;

[Authorize]
[Route("api/volunteers")]
[ApiController]
public class VolunteerApiController : ControllerBase
{
    private readonly PawHelpDbContext _context;

    public VolunteerApiController(PawHelpDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Đăng ký giúp cứu hộ bài đăng
    /// </summary>
    [HttpPost("offer")]
    public async Task<ActionResult<ApiResponse<VolunteerResponse>>> OfferHelp([FromBody] OfferHelpRequest request)
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(ApiResponse<VolunteerResponse>.ErrorResponse("Token không hợp lệ"));
        }

        // Kiểm tra bài đăng có tồn tại không
        var post = await _context.RescuePosts.FindAsync(request.PostId);
        if (post == null)
        {
            return NotFound(ApiResponse<VolunteerResponse>.ErrorResponse("Không tìm thấy bài đăng"));
        }

        // Kiểm tra đã đăng ký chưa
        var existingVolunteer = await _context.RescueVolunteers
            .FirstOrDefaultAsync(v => v.PostId == request.PostId && v.UserId == userId);

        if (existingVolunteer != null)
        {
            return BadRequest(ApiResponse<VolunteerResponse>.ErrorResponse("Bạn đã đăng ký giúp bài đăng này rồi"));
        }

        // Tạo volunteer record
        var volunteer = new RescueVolunteer
        {
            PostId = request.PostId,
            UserId = userId,
            Status = "offered",
            Message = request.Message,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _context.RescueVolunteers.Add(volunteer);
        await _context.SaveChangesAsync();

        // Tạo thông báo cho chủ bài đăng
        var user = await _context.Users.FindAsync(userId);
        var notification = new Notification
        {
            UserId = post.UserId,
            Title = "Có người tình nguyện giúp đỡ",
            Message = $"{user?.FullName} đã đăng ký giúp cứu hộ bài đăng của bạn",
            Type = "volunteer",
            RelatedPostId = post.PostId,
            Icon = "volunteer",
            IsRead = false,
            CreatedAt = DateTime.Now
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        var response = new VolunteerResponse
        {
            VolunteerId = volunteer.VolunteerId,
            PostId = volunteer.PostId,
            Status = volunteer.Status,
            Message = volunteer.Message,
            CreatedAt = volunteer.CreatedAt
        };

        return Ok(ApiResponse<VolunteerResponse>.SuccessResponse(response, "Đăng ký tình nguyện thành công"));
    }

    /// <summary>
    /// Lấy danh sách các bài đã đăng ký giúp
    /// </summary>
    [HttpGet("my-offers")]
    public async Task<ActionResult<ApiResponse<List<MyVolunteerOffer>>>> GetMyOffers()
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(ApiResponse<List<MyVolunteerOffer>>.ErrorResponse("Token không hợp lệ"));
        }

        var offers = await _context.RescueVolunteers
            .Include(v => v.Post)
            .ThenInclude(p => p.User)
            .Include(v => v.Post)
            .ThenInclude(p => p.AnimalType)
            .Where(v => v.UserId == userId)
            .OrderByDescending(v => v.CreatedAt)
            .Select(v => new MyVolunteerOffer
            {
                VolunteerId = v.VolunteerId,
                PostId = v.PostId,
                PostTitle = v.Post.Title,
                PostLocation = v.Post.Location,
                PostStatus = v.Post.Status,
                PostImageUrl = v.Post.ImageUrl,
                AnimalType = v.Post.AnimalType != null ? v.Post.AnimalType.TypeName : null,
                VolunteerStatus = v.Status,
                CreatedAt = v.CreatedAt
            })
            .ToListAsync();

        return Ok(ApiResponse<List<MyVolunteerOffer>>.SuccessResponse(offers));
    }
}

public class OfferHelpRequest
{
    public int PostId { get; set; }
    public string? Message { get; set; }
}

public class VolunteerResponse
{
    public int VolunteerId { get; set; }
    public int PostId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Message { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class MyVolunteerOffer
{
    public int VolunteerId { get; set; }
    public int PostId { get; set; }
    public string PostTitle { get; set; } = string.Empty;
    public string PostLocation { get; set; } = string.Empty;
    public string PostStatus { get; set; } = string.Empty;
    public string? PostImageUrl { get; set; }
    public string? AnimalType { get; set; }
    public string VolunteerStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

