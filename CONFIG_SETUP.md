# ⚙️ CẤU HÌNH DỰ ÁN

## 🔐 Bảo mật & Configuration

Để bảo vệ thông tin nhạy cảm, các file sau **KHÔNG được commit** lên Git:
- ✅ `appsettings.json`
- ✅ `appsettings.Development.json`
- ✅ `appsettings.Production.json`
- ✅ `wwwroot/uploads/` (file uploads)

---

## 📋 Hướng dẫn Setup

### Bước 1: Copy file cấu hình mẫu

```bash
# Windows
copy appsettings.example.json appsettings.json

# Linux/Mac
cp appsettings.example.json appsettings.json
```

### Bước 2: Cập nhật `appsettings.json`

Mở file `appsettings.json` và thay đổi:

#### 🗄️ **Connection String**

Thay `YOUR_SERVER_NAME` bằng tên SQL Server của bạn:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=pawhelp_db;..."
  }
}
```

**Các giá trị phổ biến:**
- `Server=.` - SQL Server LocalDB
- `Server=localhost` - SQL Server trên localhost
- `Server=DESKTOP-ABC123` - SQL Server với tên máy cụ thể
- `Server=localhost\\SQLEXPRESS` - SQL Server Express

**Nếu dùng SQL Authentication:**
```json
"DefaultConnection": "Server=localhost;Database=pawhelp_db;User Id=sa;Password=YourPassword;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

#### 🔑 **JWT Secret Key**

⚠️ **QUAN TRỌNG**: Thay đổi JWT Key thành chuỗi ngẫu nhiên của riêng bạn!

```json
{
  "Jwt": {
    "Key": "YOUR_RANDOM_SECRET_KEY_HERE_AT_LEAST_32_CHARACTERS"
  }
}
```

**Tạo JWT Key ngẫu nhiên:**

```bash
# PowerShell
-join ((48..57) + (65..90) + (97..122) | Get-Random -Count 50 | % {[char]$_})

# Linux/Mac
openssl rand -base64 32
```

---

## 🔒 Bảo mật Production

Khi deploy lên production:

### ✅ **Bắt buộc:**
1. Thay đổi JWT Key thành giá trị bí mật
2. Sử dụng connection string an toàn
3. Enable HTTPS only
4. Giới hạn CORS origins
5. Không expose Swagger UI

### ⚙️ **Cập nhật `Program.cs`:**

```csharp
// Chỉ enable Swagger trong Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Giới hạn CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAndroidApp", policy =>
    {
        policy.WithOrigins("https://your-production-domain.com")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

---

## 🌍 Environment Variables (Khuyến nghị cho Production)

Thay vì lưu trong file, dùng Environment Variables:

### **Azure App Service:**
```
Settings > Configuration > Application Settings
- ConnectionStrings__DefaultConnection = "..."
- Jwt__Key = "..."
```

### **Docker:**
```dockerfile
docker run -e ConnectionStrings__DefaultConnection="..." \
           -e Jwt__Key="..." \
           pawhelp-api
```

### **Đọc từ Environment trong code:**
```csharp
var jwtKey = builder.Configuration["Jwt:Key"] 
    ?? Environment.GetEnvironmentVariable("JWT_KEY");
```

---

## 📁 Tạo thư mục Uploads

```bash
mkdir -p wwwroot/uploads/posts
mkdir -p wwwroot/uploads/avatars
```

Hoặc app sẽ tự động tạo khi upload file lần đầu.

---

## ✅ Checklist Setup

- [ ] Copy `appsettings.example.json` thành `appsettings.json`
- [ ] Cập nhật Connection String
- [ ] Thay đổi JWT Key
- [ ] Chạy migrations: `dotnet ef database update`
- [ ] Test kết nối database
- [ ] Tạo thư mục uploads
- [ ] Test upload file

---

## 🆘 Troubleshooting

### Lỗi: "Cannot connect to database"
- Kiểm tra SQL Server đang chạy
- Kiểm tra tên server đúng chưa
- Test connection string bằng SQL Server Management Studio

### Lỗi: "JWT token invalid"
- Kiểm tra JWT Key giống nhau giữa generate và validate
- Kiểm tra token chưa hết hạn
- Kiểm tra format: `Bearer {token}`

### Lỗi: "Cannot write to uploads folder"
- Kiểm tra thư mục `wwwroot/uploads` tồn tại
- Kiểm tra quyền ghi (permissions)

---

## 📞 Hỗ trợ

Nếu gặp vấn đề, kiểm tra:
1. File `appsettings.json` đã tồn tại chưa
2. Connection string hợp lệ
3. SQL Server đang chạy
4. Database đã được tạo (migrations)

---

**Lưu ý:** Không bao giờ commit file `appsettings.json` lên Git!

