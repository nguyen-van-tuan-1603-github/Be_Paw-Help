using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawHelp.Data;
using PawHelp.DTOs.Common;

namespace PawHelp.Controllers.Api;

[Route("api/team")]
[ApiController]
public class TeamApiController : ControllerBase
{
    private readonly PawHelpDbContext _context;

    public TeamApiController(PawHelpDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lấy danh sách đội ngũ cứu hộ
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<TeamMemberDto>>>> GetTeamMembers()
    {
        var members = await _context.TeamMembers
            .Where(m => m.IsActive)
            .OrderBy(m => m.DisplayOrder)
            .Select(m => new TeamMemberDto
            {
                MemberId = m.MemberId,
                FullName = m.FullName,
                Role = m.Role,
                Position = m.Position,
                Description = m.Description,
                AvatarUrl = m.AvatarUrl,
                Email = m.Email,
                Phone = m.Phone,
                TeamName = m.TeamName
            })
            .ToListAsync();

        return Ok(ApiResponse<List<TeamMemberDto>>.SuccessResponse(members));
    }
}

public class TeamMemberDto
{
    public int MemberId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? Position { get; set; }
    public string? Description { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? TeamName { get; set; }
}

