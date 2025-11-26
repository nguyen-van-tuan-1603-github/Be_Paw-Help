using System.ComponentModel.DataAnnotations;

namespace PawHelp.Models.Entities;

public class Notification
{
    public int NotificationId { get; set; }
    public int UserId { get; set; }

    [Required]
    [StringLength(255)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Message { get; set; } = string.Empty;

    [StringLength(20)]
    public string Type { get; set; } = "system"; // rescue, comment, volunteer, system

    public int? RelatedPostId { get; set; }

    [StringLength(50)]
    public string? Icon { get; set; }

    public bool IsRead { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual RescuePost? RelatedPost { get; set; }
}

