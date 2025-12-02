using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawHelp.Data;
using PawHelp.DTOs.Common;
using PawHelp.DTOs.RescuePost;
using PawHelp.Models.Entities;
using PawHelp.Services;

namespace PawHelp.Controllers.Api;

[Route("api/posts")]
[ApiController]
public class RescuePostApiController : ControllerBase
{
    private readonly PawHelpDbContext _context;
    private readonly FileUploadService _fileUploadService;

    public RescuePostApiController(PawHelpDbContext context, FileUploadService fileUploadService)
    {
        _context = context;
        _fileUploadService = fileUploadService;
    }

    /// <summary>
    /// Lấy danh sách bài đăng (có phân trang)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<PostResponse>>>> GetPosts(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10,
        [FromQuery] string? status = null,
        [FromQuery] string? urgencyLevel = null,
        [FromQuery] int? animalTypeId = null)
    {
        var query = _context.RescuePosts
            .Include(p => p.User)
            .Include(p => p.AnimalType)
            .Include(p => p.PostImages)
            .Include(p => p.Comments)
            .Include(p => p.RescueVolunteers)
            .AsQueryable();

        // Filters
        if (!string.IsNullOrEmpty(status))
            query = query.Where(p => p.Status == status);

        if (!string.IsNullOrEmpty(urgencyLevel))
            query = query.Where(p => p.UrgencyLevel == urgencyLevel);

        if (animalTypeId.HasValue)
            query = query.Where(p => p.AnimalTypeId == animalTypeId);

        var total = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(total / (double)limit);

        var posts = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(p => new PostResponse
            {
                PostId = p.PostId,
                Title = p.Title,
                Description = p.Description,
                Location = p.Location,
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                ImageUrl = p.ImageUrl,
                Status = p.Status,
                UrgencyLevel = p.UrgencyLevel,
                ContactPhone = p.ContactPhone,
                ViewCount = p.ViewCount,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                AnimalType = p.AnimalType != null ? new AnimalTypeDto
                {
                    TypeId = p.AnimalType.TypeId,
                    TypeName = p.AnimalType.TypeName,
                    TypeEmoji = p.AnimalType.TypeEmoji
                } : null,
                User = new PostUserDto
                {
                    UserId = p.User.UserId,
                    FullName = p.User.FullName,
                    AvatarUrl = p.User.AvatarUrl,
                    Phone = p.User.Phone
                },
                CommentCount = p.Comments.Count,
                VolunteerCount = p.RescueVolunteers.Count,
                Images = p.PostImages.OrderBy(i => i.DisplayOrder).Select(i => i.ImageUrl).ToList()
            })
            .ToListAsync();

        var response = new PaginatedResponse<PostResponse>
        {
            Items = posts,
            Pagination = new PaginationInfo
            {
                Page = page,
                Limit = limit,
                Total = total,
                TotalPages = totalPages
            }
        };

        return Ok(ApiResponse<PaginatedResponse<PostResponse>>.SuccessResponse(response));
    }

    /// <summary>
    /// Lấy chi tiết bài đăng
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<PostResponse>>> GetPost(int id)
    {
        var post = await _context.RescuePosts
            .Include(p => p.User)
            .Include(p => p.AnimalType)
            .Include(p => p.PostImages)
            .Include(p => p.Comments)
            .Include(p => p.RescueVolunteers)
            .FirstOrDefaultAsync(p => p.PostId == id);

        if (post == null)
        {
            return NotFound(ApiResponse<PostResponse>.ErrorResponse("Không tìm thấy bài đăng"));
        }

        // Tăng view count
        post.ViewCount++;
        await _context.SaveChangesAsync();

        var response = new PostResponse
        {
            PostId = post.PostId,
            Title = post.Title,
            Description = post.Description,
            Location = post.Location,
            Latitude = post.Latitude,
            Longitude = post.Longitude,
            ImageUrl = post.ImageUrl,
            Status = post.Status,
            UrgencyLevel = post.UrgencyLevel,
            ContactPhone = post.ContactPhone,
            ViewCount = post.ViewCount,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt,
            AnimalType = post.AnimalType != null ? new AnimalTypeDto
            {
                TypeId = post.AnimalType.TypeId,
                TypeName = post.AnimalType.TypeName,
                TypeEmoji = post.AnimalType.TypeEmoji
            } : null,
            User = new PostUserDto
            {
                UserId = post.User.UserId,
                FullName = post.User.FullName,
                AvatarUrl = post.User.AvatarUrl,
                Phone = post.User.Phone
            },
            CommentCount = post.Comments.Count,
            VolunteerCount = post.RescueVolunteers.Count,
            Images = post.PostImages.OrderBy(i => i.DisplayOrder).Select(i => i.ImageUrl).ToList()
        };

        return Ok(ApiResponse<PostResponse>.SuccessResponse(response));
    }

    /// <summary>
    /// Tạo bài đăng mới (cần authentication)
    /// </summary>
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<PostResponse>>> CreatePost([FromForm] CreatePostRequest request, [FromForm] List<IFormFile>? images = null)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse<PostResponse>.ErrorResponse("Dữ liệu không hợp lệ", errors));
        }

        var userIdClaim = User.FindFirst("userId")?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(ApiResponse<PostResponse>.ErrorResponse("Token không hợp lệ"));
        }

        // Tạo post
        var post = new RescuePost
        {
            UserId = userId,
            Title = request.Title,
            Description = request.Description,
            AnimalTypeId = request.AnimalTypeId,
            Location = request.Location,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            UrgencyLevel = request.UrgencyLevel,
            ContactPhone = request.ContactPhone,
            Status = "waiting",
            ViewCount = 0,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        // Upload ảnh nếu có
        if (images != null && images.Count > 0)
        {
            var uploadResult = await _fileUploadService.UploadImagesAsync(images, "uploads/posts");
            if (uploadResult.success && uploadResult.filePaths.Count > 0)
            {
                // Ảnh đầu tiên là primary
                post.ImageUrl = uploadResult.filePaths[0];

                // Thêm vào PostImages
                for (int i = 0; i < uploadResult.filePaths.Count; i++)
                {
                    post.PostImages.Add(new PostImage
                    {
                        ImageUrl = uploadResult.filePaths[i],
                        IsPrimary = i == 0,
                        DisplayOrder = i,
                        CreatedAt = DateTime.Now
                    });
                }
            }
        }

        _context.RescuePosts.Add(post);
        await _context.SaveChangesAsync();

        // Load relations
        await _context.Entry(post).Reference(p => p.User).LoadAsync();
        await _context.Entry(post).Reference(p => p.AnimalType).LoadAsync();

        var response = new PostResponse
        {
            PostId = post.PostId,
            Title = post.Title,
            Description = post.Description,
            Location = post.Location,
            Latitude = post.Latitude,
            Longitude = post.Longitude,
            ImageUrl = post.ImageUrl,
            Status = post.Status,
            UrgencyLevel = post.UrgencyLevel,
            ContactPhone = post.ContactPhone,
            ViewCount = post.ViewCount,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt,
            AnimalType = post.AnimalType != null ? new AnimalTypeDto
            {
                TypeId = post.AnimalType.TypeId,
                TypeName = post.AnimalType.TypeName,
                TypeEmoji = post.AnimalType.TypeEmoji
            } : null,
            User = new PostUserDto
            {
                UserId = post.User.UserId,
                FullName = post.User.FullName,
                AvatarUrl = post.User.AvatarUrl,
                Phone = post.User.Phone
            },
            CommentCount = 0,
            VolunteerCount = 0,
            Images = post.PostImages.Select(i => i.ImageUrl).ToList()
        };

        return CreatedAtAction(nameof(GetPost), new { id = post.PostId }, ApiResponse<PostResponse>.SuccessResponse(response, "Tạo bài đăng thành công"));
    }

    /// <summary>
    /// Xóa bài đăng (chỉ chủ bài đăng hoặc admin)
    /// </summary>
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeletePost(int id)
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        var userRoleClaim = User.FindFirst("role")?.Value;

        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(ApiResponse<object>.ErrorResponse("Token không hợp lệ"));
        }

        var post = await _context.RescuePosts
            .Include(p => p.PostImages)
            .FirstOrDefaultAsync(p => p.PostId == id);

        if (post == null)
        {
            return NotFound(ApiResponse<object>.ErrorResponse("Không tìm thấy bài đăng"));
        }

        // Kiểm tra quyền: chỉ chủ bài hoặc admin
        if (post.UserId != userId && userRoleClaim != "admin")
        {
            return Forbid();
        }

        // Xóa ảnh
        foreach (var image in post.PostImages)
        {
            _fileUploadService.DeleteFile(image.ImageUrl);
        }

        _context.RescuePosts.Remove(post);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<object>.SuccessResponse(null, "Xóa bài đăng thành công"));
    }

    /// <summary>
    /// Lấy bài đăng của tôi
    /// </summary>
    [Authorize]
    [HttpGet("my-posts")]
    public async Task<ActionResult<ApiResponse<List<PostResponse>>>> GetMyPosts()
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(ApiResponse<List<PostResponse>>.ErrorResponse("Token không hợp lệ"));
        }

        var posts = await _context.RescuePosts
            .Include(p => p.User)
            .Include(p => p.AnimalType)
            .Include(p => p.PostImages)
            .Include(p => p.Comments)
            .Include(p => p.RescueVolunteers)
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PostResponse
            {
                PostId = p.PostId,
                Title = p.Title,
                Description = p.Description,
                Location = p.Location,
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                ImageUrl = p.ImageUrl,
                Status = p.Status,
                UrgencyLevel = p.UrgencyLevel,
                ContactPhone = p.ContactPhone,
                ViewCount = p.ViewCount,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                AnimalType = p.AnimalType != null ? new AnimalTypeDto
                {
                    TypeId = p.AnimalType.TypeId,
                    TypeName = p.AnimalType.TypeName,
                    TypeEmoji = p.AnimalType.TypeEmoji
                } : null,
                User = new PostUserDto
                {
                    UserId = p.User.UserId,
                    FullName = p.User.FullName,
                    AvatarUrl = p.User.AvatarUrl,
                    Phone = p.User.Phone
                },
                CommentCount = p.Comments.Count,
                VolunteerCount = p.RescueVolunteers.Count,
                Images = p.PostImages.OrderBy(i => i.DisplayOrder).Select(i => i.ImageUrl).ToList()
            })
            .ToListAsync();

        return Ok(ApiResponse<List<PostResponse>>.SuccessResponse(posts));
    }
}

