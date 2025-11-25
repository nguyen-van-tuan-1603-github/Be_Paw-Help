using System.ComponentModel.DataAnnotations;

namespace PawHelp.Models;

public class User
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tên người dùng là bắt buộc")]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    [StringLength(100)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Họ tên là bắt buộc")]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [StringLength(15)]
    public string? PhoneNumber { get; set; }

    [StringLength(20)]
    public string Role { get; set; } = "Staff"; // Admin, Staff, Volunteer

    public bool IsActive { get; set; } = true;

    [StringLength(200)]
    public string? Avatar { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? LastLogin { get; set; }
}

