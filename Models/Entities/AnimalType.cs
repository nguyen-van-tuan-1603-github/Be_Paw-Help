using System.ComponentModel.DataAnnotations;

namespace PawHelp.Models.Entities;

public class AnimalType
{
    public int TypeId { get; set; }

    [Required]
    [StringLength(50)]
    public string TypeName { get; set; } = string.Empty;

    [StringLength(10)]
    public string? TypeEmoji { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    public virtual ICollection<RescuePost> RescuePosts { get; set; } = new List<RescuePost>();
}

