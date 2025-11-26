using System.ComponentModel.DataAnnotations;

namespace PawHelp.Models.Entities;

public class Report
{
    public int ReportId { get; set; }
    public int PostId { get; set; }
    public int UserId { get; set; }

    [Required]
    [StringLength(20)]
    public string Reason { get; set; } = string.Empty; // fake, inappropriate, spam, scam, other

    public string? Description { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = "pending"; // pending, reviewed, resolved, rejected

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? ResolvedAt { get; set; }

    // Navigation properties
    public virtual RescuePost Post { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}

