using System.ComponentModel.DataAnnotations;

namespace PawHelp.Models;

public class Animal
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tên động vật là bắt buộc")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Loài là bắt buộc")]
    [StringLength(50)]
    public string Species { get; set; } = string.Empty; // Chó, Mèo, Chim, etc.

    [StringLength(50)]
    public string? Breed { get; set; } // Giống

    public int? Age { get; set; } // Tuổi (tháng)

    [Required]
    [StringLength(10)]
    public string Gender { get; set; } = string.Empty; // Đực, Cái

    [StringLength(500)]
    public string? Description { get; set; } // Mô tả

    [StringLength(20)]
    public string Status { get; set; } = "Đang cứu hộ"; // Đang cứu hộ, Đã điều trị, Sẵn sàng nhận nuôi, Đã được nhận nuôi

    [StringLength(200)]
    public string? ImageUrl { get; set; }

    public DateTime RescueDate { get; set; } = DateTime.Now;

    [StringLength(200)]
    public string? Location { get; set; } // Vị trí tìm thấy

    public bool IsHealthy { get; set; } = false;

    [StringLength(500)]
    public string? MedicalNotes { get; set; } // Ghi chú y tế

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
}

