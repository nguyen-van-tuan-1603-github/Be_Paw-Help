using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawHelp.Data;
using PawHelp.Models.Entities;

namespace PawHelp.Controllers;

/// <summary>
/// CONTROLLER MẪU - Demo cách sử dụng Database với Entity Framework Core
/// Controller này có thể dùng cho phần frontend người dùng sau này
/// </summary>
public class RescueController : Controller
{
    private readonly PawHelpDbContext _context;
    private readonly ILogger<RescueController> _logger;

    public RescueController(PawHelpDbContext context, ILogger<RescueController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: Rescue
    public async Task<IActionResult> Index()
    {
        // Lấy danh sách bài đăng cứu hộ với thông tin liên quan
        var posts = await _context.RescuePosts
            .Include(r => r.User)
            .Include(r => r.AnimalType)
            .Include(r => r.PostImages)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return View(posts);
    }

    // GET: Rescue/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var post = await _context.RescuePosts
            .Include(r => r.User)
            .Include(r => r.AnimalType)
            .Include(r => r.PostImages)
            .Include(r => r.Comments)
                .ThenInclude(c => c.User)
            .Include(r => r.RescueVolunteers)
                .ThenInclude(v => v.User)
            .FirstOrDefaultAsync(m => m.PostId == id);

        if (post == null)
            return NotFound();

        // Tăng view count
        post.ViewCount++;
        await _context.SaveChangesAsync();

        return View(post);
    }

    // GET: Rescue/Create
    public async Task<IActionResult> Create()
    {
        ViewBag.AnimalTypes = await _context.AnimalTypes.ToListAsync();
        return View();
    }

    // POST: Rescue/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RescuePost post)
    {
        if (ModelState.IsValid)
        {
            // Lấy user_id từ session (giả sử đã đăng nhập)
            var userId = 1; // Hardcode for demo, should get from session

            post.UserId = userId;
            post.CreatedAt = DateTime.Now;
            post.UpdatedAt = DateTime.Now;

            _context.Add(post);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Tạo bài đăng cứu hộ thành công!";
            return RedirectToAction(nameof(Details), new { id = post.PostId });
        }

        ViewBag.AnimalTypes = await _context.AnimalTypes.ToListAsync();
        return View(post);
    }

    // POST: Rescue/AddComment
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment(int postId, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            TempData["Error"] = "Nội dung bình luận không được để trống!";
            return RedirectToAction(nameof(Details), new { id = postId });
        }

        var userId = 1; // Should get from session

        var comment = new Comment
        {
            PostId = postId,
            UserId = userId,
            Content = content,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Đã thêm bình luận!";
        return RedirectToAction(nameof(Details), new { id = postId });
    }

    // POST: Rescue/Volunteer
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult>Volunteer(int postId, string message)
    {
        var userId = 1; // Should get from session

        // Kiểm tra đã đăng ký chưa
        var exists = await _context.RescueVolunteers
            .AnyAsync(v => v.PostId == postId && v.UserId == userId);

        if (exists)
        {
            TempData["Error"] = "Bạn đã đăng ký tình nguyện cho bài đăng này rồi!";
            return RedirectToAction(nameof(Details), new { id = postId });
        }

        var volunteer = new RescueVolunteer
        {
            PostId = postId,
            UserId = userId,
            Message = message,
            Status = "offered",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _context.RescueVolunteers.Add(volunteer);

        // Tạo notification cho chủ bài đăng
        var post = await _context.RescuePosts.FindAsync(postId);
        if (post != null)
        {
            var notification = new Notification
            {
                UserId = post.UserId,
                Title = "Có tình nguyện viên mới!",
                Message = $"Có người đăng ký giúp cứu hộ bài đăng: {post.Title}",
                Type = "volunteer",
                RelatedPostId = postId,
                Icon = "volunteer",
                CreatedAt = DateTime.Now
            };
            _context.Notifications.Add(notification);
        }

        await _context.SaveChangesAsync();

        TempData["Success"] = "Đăng ký tình nguyện thành công!";
        return RedirectToAction(nameof(Details), new { id = postId });
    }

    // GET: Rescue/MyPosts
    public async Task<IActionResult> MyPosts()
    {
        var userId = 1; // Should get from session

        var posts = await _context.RescuePosts
            .Include(r => r.AnimalType)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return View(posts);
    }

    // GET: Rescue/Statistics
    public async Task<IActionResult> Statistics()
    {
        var stats = new
        {
            TotalPosts = await _context.RescuePosts.CountAsync(),
            WaitingPosts = await _context.RescuePosts.CountAsync(r => r.Status == "waiting"),
            RescuedPosts = await _context.RescuePosts.CountAsync(r => r.Status == "rescued"),
            TotalVolunteers = await _context.Users.CountAsync(u => u.UserRole == "volunteer"),
            TotalDonations = await _context.Donations
                .Where(d => d.Status == "completed")
                .SumAsync(d => (decimal?)d.Amount) ?? 0
        };

        return Json(stats);
    }
}

