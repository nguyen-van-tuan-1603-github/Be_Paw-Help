using System.ComponentModel.DataAnnotations;

namespace PawHelp.Models.Entities;

public class User
{
    public int UserId { get; set; }

    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Phone]
    [StringLength(20)]
    public string? Phone { get; set; }

    [Required]
    [StringLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [StringLength(255)]
    public string? AvatarUrl { get; set; }

    [StringLength(20)]
    public string UserRole { get; set; } = "user"; // user, volunteer, admin

    [StringLength(20)]
    public string Status { get; set; } = "active"; // active, inactive, banned

    public bool EmailVerified { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public DateTime? LastLogin { get; set; }

    // Navigation properties
    public virtual ICollection<RescuePost> RescuePosts { get; set; } = new List<RescuePost>();
    public virtual ICollection<RescueVolunteer> RescueVolunteers { get; set; } = new List<RescueVolunteer>();
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}

