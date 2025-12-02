namespace PawHelp.Services;

public class FileUploadService
{
    private readonly IWebHostEnvironment _environment;
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

    public FileUploadService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    /// <summary>
    /// Upload một file ảnh
    /// </summary>
    public async Task<(bool success, string? filePath, string? error)> UploadImageAsync(IFormFile file, string folder = "uploads")
    {
        if (file == null || file.Length == 0)
            return (false, null, "File không hợp lệ");

        // Kiểm tra kích thước
        if (file.Length > MaxFileSize)
            return (false, null, "File vượt quá 5MB");

        // Kiểm tra extension
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_allowedExtensions.Contains(extension))
            return (false, null, "Chỉ chấp nhận file ảnh (jpg, jpeg, png, gif, webp)");

        try
        {
            // Tạo tên file unique
            var fileName = $"{Guid.NewGuid()}{extension}";
            
            // Tạo đường dẫn thư mục
            var uploadPath = Path.Combine(_environment.WebRootPath, folder);
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            // Đường dẫn đầy đủ
            var filePath = Path.Combine(uploadPath, fileName);

            // Lưu file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Trả về đường dẫn relative
            return (true, $"/{folder}/{fileName}", null);
        }
        catch (Exception ex)
        {
            return (false, null, $"Lỗi khi upload: {ex.Message}");
        }
    }

    /// <summary>
    /// Upload nhiều file ảnh
    /// </summary>
    public async Task<(bool success, List<string> filePaths, List<string> errors)> UploadImagesAsync(List<IFormFile> files, string folder = "uploads")
    {
        var filePaths = new List<string>();
        var errors = new List<string>();

        foreach (var file in files)
        {
            var result = await UploadImageAsync(file, folder);
            if (result.success && result.filePath != null)
            {
                filePaths.Add(result.filePath);
            }
            else if (result.error != null)
            {
                errors.Add($"{file.FileName}: {result.error}");
            }
        }

        return (filePaths.Count > 0, filePaths, errors);
    }

    /// <summary>
    /// Xóa file
    /// </summary>
    public bool DeleteFile(string relativeFilePath)
    {
        try
        {
            var filePath = Path.Combine(_environment.WebRootPath, relativeFilePath.TrimStart('/'));
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }
}

