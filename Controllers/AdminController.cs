using Microsoft.AspNetCore.Mvc;
using PawHelp.Models;

namespace PawHelp.Controllers;

public class AdminController : Controller
{
    private readonly ILogger<AdminController> _logger;

    // Dữ liệu tạm thời (trong thực tế sẽ dùng Database)
    private static List<Animal> _animals = new List<Animal>
    {
        new Animal 
        { 
            Id = 1, 
            Name = "Bobby", 
            Species = "Chó", 
            Breed = "Golden Retriever",
            Age = 24,
            Gender = "Đực",
            Status = "Đang cứu hộ",
            Description = "Chó bị bỏ rơi, tình trạng sức khỏe ổn định",
            Location = "Quận 1, TP.HCM",
            RescueDate = DateTime.Now.AddDays(-5)
        },
        new Animal 
        { 
            Id = 2, 
            Name = "Mimi", 
            Species = "Mèo", 
            Breed = "Mèo ta",
            Age = 6,
            Gender = "Cái",
            Status = "Sẵn sàng nhận nuôi",
            Description = "Mèo con dễ thương, đã được tiêm phòng",
            Location = "Quận 3, TP.HCM",
            IsHealthy = true,
            RescueDate = DateTime.Now.AddDays(-15)
        }
    };

    private static List<RescueRequest> _requests = new List<RescueRequest>
    {
        new RescueRequest
        {
            Id = 1,
            ReporterName = "Nguyễn Văn A",
            PhoneNumber = "0912345678",
            Email = "nguyenvana@email.com",
            Location = "123 Đường ABC, Quận 5, TP.HCM",
            Description = "Có một chú chó bị thương ở đầu đường, cần cứu hộ gấp",
            AnimalType = "Chó",
            Priority = "Khẩn cấp",
            Status = "Chờ xử lý",
            RequestDate = DateTime.Now.AddHours(-2)
        },
        new RescueRequest
        {
            Id = 2,
            ReporterName = "Trần Thị B",
            PhoneNumber = "0987654321",
            Location = "456 Đường XYZ, Quận 7, TP.HCM",
            Description = "Mèo con bị mắc kẹt trên cây cao khoảng 3m",
            AnimalType = "Mèo",
            Priority = "Cao",
            Status = "Đang xử lý",
            RequestDate = DateTime.Now.AddHours(-5),
            ResponseDate = DateTime.Now.AddHours(-3)
        }
    };

    private static List<User> _users = new List<User>
    {
        new User
        {
            Id = 1,
            Username = "admin",
            Email = "admin@pawhelp.com",
            FullName = "Quản trị viên",
            PhoneNumber = "0901234567",
            Role = "Admin",
            IsActive = true
        },
        new User
        {
            Id = 2,
            Username = "staff01",
            Email = "staff01@pawhelp.com",
            FullName = "Nhân viên 1",
            PhoneNumber = "0909876543",
            Role = "Staff",
            IsActive = true
        }
    };

    public AdminController(ILogger<AdminController> logger)
    {
        _logger = logger;
    }

    // Dashboard
    public IActionResult Index()
    {
        ViewBag.TotalAnimals = _animals.Count;
        ViewBag.AnimalsInRescue = _animals.Count(a => a.Status == "Đang cứu hộ");
        ViewBag.AnimalsReadyForAdoption = _animals.Count(a => a.Status == "Sẵn sàng nhận nuôi");
        ViewBag.PendingRequests = _requests.Count(r => r.Status == "Chờ xử lý");
        ViewBag.TotalRequests = _requests.Count;
        ViewBag.ActiveUsers = _users.Count(u => u.IsActive);

        ViewBag.RecentRequests = _requests.OrderByDescending(r => r.RequestDate).Take(5).ToList();
        ViewBag.RecentAnimals = _animals.OrderByDescending(a => a.RescueDate).Take(5).ToList();

        return View();
    }

    // ===== ANIMALS =====
    public IActionResult Animals()
    {
        return View(_animals.OrderByDescending(a => a.CreatedAt).ToList());
    }

    public IActionResult AnimalDetails(int id)
    {
        var animal = _animals.FirstOrDefault(a => a.Id == id);
        if (animal == null)
            return NotFound();

        return View(animal);
    }

    [HttpGet]
    public IActionResult CreateAnimal()
    {
        return View();
    }

    [HttpPost]
    public IActionResult CreateAnimal(Animal animal)
    {
        if (ModelState.IsValid)
        {
            animal.Id = _animals.Any() ? _animals.Max(a => a.Id) + 1 : 1;
            animal.CreatedAt = DateTime.Now;
            _animals.Add(animal);
            TempData["Success"] = "Thêm động vật thành công!";
            return RedirectToAction(nameof(Animals));
        }
        return View(animal);
    }

    [HttpGet]
    public IActionResult EditAnimal(int id)
    {
        var animal = _animals.FirstOrDefault(a => a.Id == id);
        if (animal == null)
            return NotFound();

        return View(animal);
    }

    [HttpPost]
    public IActionResult EditAnimal(Animal animal)
    {
        if (ModelState.IsValid)
        {
            var existingAnimal = _animals.FirstOrDefault(a => a.Id == animal.Id);
            if (existingAnimal == null)
                return NotFound();

            existingAnimal.Name = animal.Name;
            existingAnimal.Species = animal.Species;
            existingAnimal.Breed = animal.Breed;
            existingAnimal.Age = animal.Age;
            existingAnimal.Gender = animal.Gender;
            existingAnimal.Description = animal.Description;
            existingAnimal.Status = animal.Status;
            existingAnimal.Location = animal.Location;
            existingAnimal.IsHealthy = animal.IsHealthy;
            existingAnimal.MedicalNotes = animal.MedicalNotes;
            existingAnimal.UpdatedAt = DateTime.Now;

            TempData["Success"] = "Cập nhật động vật thành công!";
            return RedirectToAction(nameof(Animals));
        }
        return View(animal);
    }

    [HttpPost]
    public IActionResult DeleteAnimal(int id)
    {
        var animal = _animals.FirstOrDefault(a => a.Id == id);
        if (animal != null)
        {
            _animals.Remove(animal);
            TempData["Success"] = "Xóa động vật thành công!";
        }
        return RedirectToAction(nameof(Animals));
    }

    // ===== RESCUE REQUESTS =====
    public IActionResult RescueRequests()
    {
        return View(_requests.OrderByDescending(r => r.RequestDate).ToList());
    }

    public IActionResult RequestDetails(int id)
    {
        var request = _requests.FirstOrDefault(r => r.Id == id);
        if (request == null)
            return NotFound();

        ViewBag.Users = _users.Where(u => u.IsActive).ToList();
        return View(request);
    }

    [HttpPost]
    public IActionResult UpdateRequestStatus(int id, string status, string? adminNotes)
    {
        var request = _requests.FirstOrDefault(r => r.Id == id);
        if (request != null)
        {
            request.Status = status;
            request.AdminNotes = adminNotes;
            request.UpdatedAt = DateTime.Now;

            if (status == "Đang xử lý" && request.ResponseDate == null)
                request.ResponseDate = DateTime.Now;
            
            if (status == "Hoàn thành")
                request.CompletedDate = DateTime.Now;

            TempData["Success"] = "Cập nhật trạng thái thành công!";
        }
        return RedirectToAction(nameof(RequestDetails), new { id });
    }

    [HttpPost]
    public IActionResult AssignRequest(int id, int userId)
    {
        var request = _requests.FirstOrDefault(r => r.Id == id);
        if (request != null)
        {
            request.AssignedToUserId = userId;
            request.UpdatedAt = DateTime.Now;
            TempData["Success"] = "Phân công nhiệm vụ thành công!";
        }
        return RedirectToAction(nameof(RequestDetails), new { id });
    }

    // ===== USERS =====
    public IActionResult Users()
    {
        return View(_users.OrderBy(u => u.Username).ToList());
    }

    public IActionResult UserDetails(int id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user == null)
            return NotFound();

        return View(user);
    }

    [HttpGet]
    public IActionResult CreateUser()
    {
        return View();
    }

    [HttpPost]
    public IActionResult CreateUser(User user)
    {
        if (ModelState.IsValid)
        {
            user.Id = _users.Any() ? _users.Max(u => u.Id) + 1 : 1;
            user.CreatedAt = DateTime.Now;
            _users.Add(user);
            TempData["Success"] = "Thêm người dùng thành công!";
            return RedirectToAction(nameof(Users));
        }
        return View(user);
    }

    [HttpGet]
    public IActionResult EditUser(int id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user == null)
            return NotFound();

        return View(user);
    }

    [HttpPost]
    public IActionResult EditUser(User user)
    {
        if (ModelState.IsValid)
        {
            var existingUser = _users.FirstOrDefault(u => u.Id == user.Id);
            if (existingUser == null)
                return NotFound();

            existingUser.Username = user.Username;
            existingUser.Email = user.Email;
            existingUser.FullName = user.FullName;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.Role = user.Role;
            existingUser.IsActive = user.IsActive;

            TempData["Success"] = "Cập nhật người dùng thành công!";
            return RedirectToAction(nameof(Users));
        }
        return View(user);
    }

    [HttpPost]
    public IActionResult ToggleUserStatus(int id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user != null)
        {
            user.IsActive = !user.IsActive;
            TempData["Success"] = $"Đã {(user.IsActive ? "kích hoạt" : "vô hiệu hóa")} người dùng!";
        }
        return RedirectToAction(nameof(Users));
    }
}

