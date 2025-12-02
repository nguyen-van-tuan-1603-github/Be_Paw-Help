using System.ComponentModel.DataAnnotations;

namespace PawHelp.DTOs.RescuePost;

public class CreatePostRequest
{
    [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
    [StringLength(255)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mô tả là bắt buộc")]
    public string Description { get; set; } = string.Empty;

    public int? AnimalTypeId { get; set; }

    [Required(ErrorMessage = "Địa điểm là bắt buộc")]
    [StringLength(255)]
    public string Location { get; set; } = string.Empty;

    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    [StringLength(20)]
    public string UrgencyLevel { get; set; } = "medium"; // low, medium, high, critical

    [Phone]
    [StringLength(20)]
    public string? ContactPhone { get; set; }

    // Files sẽ được upload riêng qua multipart/form-data
}

