using System.ComponentModel.DataAnnotations;

namespace PawHelp.Models.Entities;

public class Donation
{
    public int DonationId { get; set; }
    public int? UserId { get; set; }

    [StringLength(100)]
    public string? DonorName { get; set; }

    [EmailAddress]
    [StringLength(100)]
    public string? DonorEmail { get; set; }

    [Phone]
    [StringLength(20)]
    public string? DonorPhone { get; set; }

    [Required]
    public decimal Amount { get; set; }

    public string? Message { get; set; }

    [Required]
    [StringLength(20)]
    public string PaymentMethod { get; set; } = string.Empty; // momo, bank_transfer, cash, other

    [StringLength(100)]
    public string? TransactionId { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = "pending"; // pending, completed, failed, refunded

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? CompletedAt { get; set; }

    // Navigation properties
    public virtual User? User { get; set; }
}

