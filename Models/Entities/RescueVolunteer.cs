namespace PawHelp.Models.Entities;

public class RescueVolunteer
{
    public int VolunteerId { get; set; }
    public int PostId { get; set; }
    public int UserId { get; set; }

    public string Status { get; set; } = "offered"; // offered, accepted, declined, completed

    public string? Message { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    public virtual RescuePost Post { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}

