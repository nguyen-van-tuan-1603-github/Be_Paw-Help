using System.ComponentModel.DataAnnotations;

namespace PawHelp.Models.Entities;

public class PostImage
{
    public int ImageId { get; set; }
    public int PostId { get; set; }

    [Required]
    [StringLength(255)]
    public string ImageUrl { get; set; } = string.Empty;

    public bool IsPrimary { get; set; } = false;
    public int DisplayOrder { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    public virtual RescuePost Post { get; set; } = null!;
}

