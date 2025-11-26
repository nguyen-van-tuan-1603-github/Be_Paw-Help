using System.ComponentModel.DataAnnotations;

namespace PawHelp.Models.Entities;

public class RescuePost
{
    public int PostId { get; set; }

    public int UserId { get; set; }

    [Required]
    [StringLength(255)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    public int? AnimalTypeId { get; set; }

    [Required]
    [StringLength(255)]
    public string Location { get; set; } = string.Empty;

    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    [StringLength(255)]
    public string? ImageUrl { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = "waiting"; // waiting, processing, rescued, cancelled

    [StringLength(20)]
    public string UrgencyLevel { get; set; } = "medium"; // low, medium, high, critical

    [StringLength(20)]
    public string? ContactPhone { get; set; }

    public int ViewCount { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public DateTime? RescuedAt { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual AnimalType? AnimalType { get; set; }
    public virtual ICollection<RescueVolunteer> RescueVolunteers { get; set; } = new List<RescueVolunteer>();
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public virtual ICollection<PostImage> PostImages { get; set; } = new List<PostImage>();
}

