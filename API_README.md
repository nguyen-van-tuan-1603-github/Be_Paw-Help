# 🐾 PAW-HELP BACKEND API

Backend API cho ứng dụng Android **Paw-help** - Hệ thống cứu hộ động vật tại Đà Nẵng.

## 🎯 Tính năng

✅ **Authentication**: JWT-based với Register/Login  
✅ **Rescue Posts**: CRUD bài đăng cứu hộ, upload ảnh, phân trang  
✅ **User Profile**: Quản lý thông tin cá nhân, upload avatar  
✅ **Notifications**: Hệ thống thông báo real-time  
✅ **Volunteers**: Đăng ký tình nguyện giúp cứu hộ  
✅ **Dashboard**: Thống kê SOS, đã cứu, tổng số  
✅ **Team Management**: Danh sách đội ngũ cứu hộ  
✅ **CORS**: Cho phép Android app gọi API  
✅ **Swagger UI**: Test API trực tiếp trong trình duyệt  

---

## 🚀 Hướng dẫn chạy

### 1. Yêu cầu
- **.NET SDK 9.0** trở lên
- **SQL Server** (LocalDB hoặc SQL Server Express)
- **Visual Studio 2022** hoặc **VS Code** (optional)

### 2. Clone & Restore packages
```bash
cd BE_Paw-help
dotnet restore
```

### 3. Cấu hình Database

Mở `appsettings.json` và kiểm tra connection string:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=pawhelp_db;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=true;Encrypt=False"
  }
}
```

### 4. Chạy Migrations (Tạo database)
```bash
dotnet ef database update
```

### 5. Chạy API
```bash
dotnet run
```

API sẽ chạy tại:
- HTTPS: `https://localhost:7XXX` (port tự động)
- HTTP: `http://localhost:5XXX`

### 6. Truy cập Swagger UI
Mở trình duyệt:
```
https://localhost:7XXX/api/docs
```

---

## 📁 Cấu trúc thư mục

```
BE_Paw-help/
├── Controllers/
│   ├── AdminController.cs          # Web Admin (MVC)
│   ├── AuthController.cs           # Web Admin Login
│   ├── RescueController.cs         # Web Admin CRUD
│   └── Api/                        # ✨ Mobile API
│       ├── AuthApiController.cs
│       ├── RescuePostApiController.cs
│       ├── UserApiController.cs
│       ├── NotificationApiController.cs
│       ├── VolunteerApiController.cs
│       ├── DashboardApiController.cs
│       └── TeamApiController.cs
├── Models/
│   └── Entities/                   # Database Models
│       ├── User.cs
│       ├── RescuePost.cs
│       ├── AnimalType.cs
│       ├── Notification.cs
│       ├── Comment.cs
│       ├── RescueVolunteer.cs
│       ├── Report.cs
│       ├── TeamMember.cs
│       ├── Donation.cs
│       └── ...
├── DTOs/                           # Data Transfer Objects
│   ├── Auth/
│   │   ├── RegisterRequest.cs
│   │   ├── LoginRequest.cs
│   │   └── AuthResponse.cs
│   ├── RescuePost/
│   │   ├── CreatePostRequest.cs
│   │   └── PostResponse.cs
│   ├── User/
│   │   └── UpdateProfileRequest.cs
│   └── Common/
│       └── ApiResponse.cs
├── Services/                       # Business Logic
│   ├── JwtService.cs               # JWT token generation
│   ├── PasswordService.cs          # BCrypt password hashing
│   └── FileUploadService.cs        # File upload handling
├── Data/
│   └── PawHelpDbContext.cs         # EF Core DbContext
├── Middleware/
│   └── AdminAuthenticationMiddleware.cs
├── Views/                          # Razor Pages (Web Admin)
├── wwwroot/                        # Static files & uploads
│   └── uploads/
│       ├── posts/                  # Ảnh bài đăng
│       └── avatars/                # Ảnh avatar
├── Program.cs                      # App configuration
├── appsettings.json                # Configuration
└── API_DOCUMENTATION.md            # API docs chi tiết
```

---

## 🔧 Cấu hình quan trọng

### JWT Configuration (`appsettings.json`)
```json
{
  "Jwt": {
    "Key": "YourSuperSecretKeyForPawHelpApplication2025AtLeast32Characters",
    "Issuer": "PawHelpAPI",
    "Audience": "PawHelpAndroidApp",
    "ExpiryDays": 7
  }
}
```

**⚠️ Quan trọng**: Trong production, đổi `Jwt:Key` thành secret key riêng!

### CORS Configuration
CORS đã được cấu hình cho phép tất cả origins. Trong production, nên giới hạn:

```csharp
// Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAndroidApp", policy =>
    {
        policy.WithOrigins("https://your-android-app-domain.com")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

---

## 🧪 Test API

### Sử dụng Swagger UI
1. Chạy API: `dotnet run`
2. Mở `https://localhost:7XXX/api/docs`
3. Click nút **"Authorize"** ở góc phải
4. Nhập: `Bearer {your_jwt_token}`
5. Test các endpoints

### Sử dụng Postman/Thunder Client

**1. Register:**
```http
POST https://localhost:7XXX/api/auth/register
Content-Type: application/json

{
  "fullName": "Nguyễn Văn A",
  "email": "test@example.com",
  "phone": "0123456789",
  "password": "password123",
  "confirmPassword": "password123"
}
```

**2. Login:**
```http
POST https://localhost:7XXX/api/auth/login
Content-Type: application/json

{
  "email": "test@example.com",
  "password": "password123"
}
```

Copy `token` từ response.

**3. Get Posts:**
```http
GET https://localhost:7XXX/api/posts?page=1&limit=10
```

**4. Create Post (với Auth):**
```http
POST https://localhost:7XXX/api/posts
Authorization: Bearer {your_token}
Content-Type: multipart/form-data

title: "Chó con cần cứu"
description: "Mô tả..."
location: "Đà Nẵng"
urgencyLevel: "high"
images: [file1, file2]
```

---

## 📱 Kết nối với Android App

### 1. Thêm dependencies (Retrofit)
```gradle
// app/build.gradle
dependencies {
    implementation 'com.squareup.retrofit2:retrofit:2.9.0'
    implementation 'com.squareup.retrofit2:converter-gson:2.9.0'
    implementation 'com.squareup.okhttp3:logging-interceptor:4.11.0'
}
```

### 2. Tạo API Interface
```java
public interface PawHelpApi {
    @POST("auth/login")
    Call<ApiResponse<AuthResponse>> login(@Body LoginRequest request);
    
    @GET("posts")
    Call<ApiResponse<PaginatedResponse<PostResponse>>> getPosts();
}
```

### 3. Setup Retrofit
```java
String BASE_URL = "https://your-server-ip:7XXX/api/";

Retrofit retrofit = new Retrofit.Builder()
    .baseUrl(BASE_URL)
    .addConverterFactory(GsonConverterFactory.create())
    .build();

PawHelpApi api = retrofit.create(PawHelpApi.class);
```

### 4. Lưu JWT Token
```java
// Sau khi login thành công
SharedPreferences prefs = getSharedPreferences("PawHelp", MODE_PRIVATE);
prefs.edit().putString("jwt_token", response.getData().getToken()).apply();

// Thêm token vào header
OkHttpClient client = new OkHttpClient.Builder()
    .addInterceptor(chain -> {
        String token = prefs.getString("jwt_token", "");
        Request request = chain.request().newBuilder()
            .addHeader("Authorization", "Bearer " + token)
            .build();
        return chain.proceed(request);
    })
    .build();
```

---

## 🗄️ Database Schema

Các bảng chính:
- `users` - Người dùng (app + admin)
- `rescue_posts` - Bài đăng cứu hộ
- `animal_types` - Loại động vật
- `rescue_volunteers` - Đăng ký tình nguyện
- `comments` - Bình luận
- `notifications` - Thông báo
- `reports` - Báo cáo vi phạm
- `team_members` - Đội ngũ
- `post_images` - Ảnh bài đăng

---

## 🔐 Bảo mật

✅ Password được hash bằng **BCrypt**  
✅ JWT token hết hạn sau **7 ngày**  
✅ File upload chỉ chấp nhận **image types**  
✅ File size giới hạn **5MB**  
✅ HTTPS only (production)  
✅ Input validation với **Data Annotations**  
✅ Authorization check (chỉ owner mới xóa/sửa bài đăng)  

---

## 🐛 Troubleshooting

### Lỗi: "Unable to connect to database"
- Kiểm tra SQL Server đã chạy chưa
- Kiểm tra connection string trong `appsettings.json`
- Chạy lại migrations: `dotnet ef database update`

### Lỗi: "CORS policy blocked"
- Kiểm tra CORS đã enable trong `Program.cs`
- Kiểm tra `app.UseCors("AllowAndroidApp")` đặt trước `UseAuthorization()`

### Lỗi: "401 Unauthorized"
- Kiểm tra JWT token có hợp lệ không
- Kiểm tra header: `Authorization: Bearer {token}`
- Token có thể đã hết hạn (7 ngày)

### File upload không hoạt động
- Kiểm tra thư mục `wwwroot/uploads` đã tồn tại
- Kiểm tra quyền ghi file
- Kiểm tra file size < 5MB
- Kiểm tra file extension (jpg, jpeg, png, gif, webp)

---

## 📊 Performance Tips

1. **Database Indexing**: Đã được cấu hình trong `DbContext`
2. **Caching**: Có thể thêm Redis cache cho endpoints thường xuyên
3. **Pagination**: Luôn dùng `page` & `limit` cho danh sách lớn
4. **Image Optimization**: Nên resize ảnh trước khi lưu (future enhancement)

---

## 🚢 Deploy lên Production

### Option 1: Azure App Service
```bash
dotnet publish -c Release
# Upload to Azure App Service
```

### Option 2: Docker
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY bin/Release/net9.0/publish/ .
ENTRYPOINT ["dotnet", "PawHelp.dll"]
```

### Option 3: IIS
1. Publish: `dotnet publish -c Release`
2. Copy folder `bin/Release/net9.0/publish/` to IIS
3. Setup Application Pool (.NET Core)

**Nhớ:**
- Đổi `Jwt:Key` trong production
- Enable HTTPS
- Giới hạn CORS origins
- Setup backup database

---

## 📖 Tài liệu đầy đủ

Xem file `API_DOCUMENTATION.md` để biết chi tiết tất cả endpoints, request/response formats, và examples.

---

## 👨‍💻 Development

### Run in Development mode
```bash
dotnet run --environment Development
```

### Run in Production mode
```bash
dotnet run --environment Production
```

### Hot Reload (auto restart khi code thay đổi)
```bash
dotnet watch run
```

---

## 📞 Liên hệ

**Hội Cứu Trợ Động Vật Đà Nẵng**
- 📧 Email: contact@pawhelp.vn
- 📱 Hotline: 113
- 🌐 Website: www.pawhelp.vn

---

## 📄 License

Dự án này được phát triển cho mục đích giáo dục và phi lợi nhuận.

**"Yêu thương và hành động - cùng nhau tạo nên sự khác biệt!"** 🐾

