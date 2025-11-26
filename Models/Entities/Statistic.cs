using System.ComponentModel.DataAnnotations;

namespace PawHelp.Models.Entities;

public class Statistic
{
    public int StatId { get; set; }

    [Required]
    public DateTime StatDate { get; set; }

    public int TotalRescues { get; set; } = 0;
    public int PendingRescues { get; set; } = 0;
    public int CompletedRescues { get; set; } = 0;
    public int TotalUsers { get; set; } = 0;
    public int ActiveVolunteers { get; set; } = 0;
    public decimal TotalDonations { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

