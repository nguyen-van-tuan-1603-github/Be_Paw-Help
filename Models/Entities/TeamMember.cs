using System.ComponentModel.DataAnnotations;

namespace PawHelp.Models.Entities;

public class TeamMember
{
    public int MemberId { get; set; }

    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Role { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Position { get; set; }

    public string? Description { get; set; }

    [StringLength(255)]
    public string? AvatarUrl { get; set; }

    [EmailAddress]
    [StringLength(100)]
    public string? Email { get; set; }

    [Phone]
    [StringLength(20)]
    public string? Phone { get; set; }

    [StringLength(100)]
    public string? TeamName { get; set; }

    public int DisplayOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

