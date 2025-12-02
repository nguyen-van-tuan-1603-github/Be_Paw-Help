namespace PawHelp.DTOs.Auth;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public UserInfo User { get; set; } = new();
}

public class UserInfo
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? AvatarUrl { get; set; }
    public string UserRole { get; set; } = "user";
    public string Status { get; set; } = "active";
    public bool EmailVerified { get; set; }
    public DateTime CreatedAt { get; set; }
}

