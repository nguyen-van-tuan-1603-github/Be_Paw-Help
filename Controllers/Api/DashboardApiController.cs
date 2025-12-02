using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawHelp.Data;
using PawHelp.DTOs.Common;

namespace PawHelp.Controllers.Api;

[Route("api/dashboard")]
[ApiController]
public class DashboardApiController : ControllerBase
{
    private readonly PawHelpDbContext _context;

    public DashboardApiController(PawHelpDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lấy thống kê cho trang chủ (SOS, đã cứu, tổng số)
    /// </summary>
    [HttpGet("stats")]
    public async Task<ActionResult<ApiResponse<DashboardStats>>> GetStats()
    {
        var sosCount = await _context.RescuePosts.CountAsync(p => p.Status == "waiting" && p.UrgencyLevel == "critical");
        var rescuedCount = await _context.RescuePosts.CountAsync(p => p.Status == "rescued");
        var totalPosts = await _context.RescuePosts.CountAsync();
        var activeVolunteers = await _context.RescueVolunteers.Select(v => v.UserId).Distinct().CountAsync();

        var stats = new DashboardStats
        {
            SosCount = sosCount,
            RescuedCount = rescuedCount,
            TotalPosts = totalPosts,
            ActiveVolunteers = activeVolunteers
        };

        return Ok(ApiResponse<DashboardStats>.SuccessResponse(stats));
    }
}

public class DashboardStats
{
    public int SosCount { get; set; }
    public int RescuedCount { get; set; }
    public int TotalPosts { get; set; }
    public int ActiveVolunteers { get; set; }
}

