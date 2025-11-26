using System.ComponentModel.DataAnnotations;

namespace PawHelp.Models.Entities;

public class RescueHistory
{
    public int HistoryId { get; set; }
    public int PostId { get; set; }
    public int VolunteerId { get; set; }

    [Required]
    [StringLength(20)]
    public string Action { get; set; } = string.Empty; // offered, accepted, started, completed, cancelled

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    public virtual RescuePost Post { get; set; } = null!;
    public virtual User Volunteer { get; set; } = null!;
}

