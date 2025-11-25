using System.ComponentModel.DataAnnotations;

namespace PawHelp.Models;

public class RescueRequest
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tên người báo cáo là bắt buộc")]
    [StringLength(100)]
    public string ReporterName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [StringLength(15)]
    public string PhoneNumber { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [StringLength(100)]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Địa điểm là bắt buộc")]
    [StringLength(300)]
    public string Location { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mô tả tình huống là bắt buộc")]
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [StringLength(50)]
    public string? AnimalType { get; set; } // Loại động vật

    [StringLength(20)]
    public string Priority { get; set; } = "Trung bình"; // Khẩn cấp, Cao, Trung bình, Thấp

    [StringLength(20)]
    public string Status { get; set; } = "Chờ xử lý"; // Chờ xử lý, Đang xử lý, Hoàn thành, Hủy bỏ

    public DateTime RequestDate { get; set; } = DateTime.Now;

    public DateTime? ResponseDate { get; set; }

    public DateTime? CompletedDate { get; set; }

    [StringLength(500)]
    public string? AdminNotes { get; set; } // Ghi chú của admin

    public int? AssignedToUserId { get; set; }

    [StringLength(200)]
    public string? ImageUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
}

