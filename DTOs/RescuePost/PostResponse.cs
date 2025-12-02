namespace PawHelp.DTOs.RescuePost;

public class PostResponse
{
    public int PostId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? ImageUrl { get; set; }
    public string Status { get; set; } = "waiting";
    public string UrgencyLevel { get; set; } = "medium";
    public string? ContactPhone { get; set; }
    public int ViewCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public AnimalTypeDto? AnimalType { get; set; }
    public PostUserDto User { get; set; } = new();
    
    public int CommentCount { get; set; }
    public int VolunteerCount { get; set; }
    public List<string> Images { get; set; } = new();
}

public class AnimalTypeDto
{
    public int TypeId { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public string? TypeEmoji { get; set; }
}

public class PostUserDto
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? Phone { get; set; }
}

