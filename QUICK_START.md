# 🚀 QUICK START - PAW-HELP API

## ✅ API đã được cấu hình xong!

Backend API cho Android app **Paw-help** đã sẵn sàng sử dụng.

---

## 🎯 Những gì đã hoàn thành

✅ **JWT Authentication** - Đăng ký/Đăng nhập với JWT token  
✅ **Rescue Posts API** - CRUD bài đăng cứu hộ, upload ảnh  
✅ **User Profile API** - Quản lý profile, upload avatar  
✅ **Notifications API** - Thông báo cho user  
✅ **Volunteers API** - Đăng ký tình nguyện  
✅ **Dashboard API** - Thống kê tổng quan  
✅ **Team API** - Danh sách đội ngũ  
✅ **Services** - JwtService, PasswordService, FileUploadService  
✅ **CORS** - Đã enable cho Android app  
✅ **Swagger UI** - Test API trực tiếp  

---

## 🌐 URL API hiện tại

API đang chạy tại:
```
http://localhost:5125/api
```

**Swagger UI (Test API):**
```
http://localhost:5125/api/docs
```

---

## 📋 Danh sách API Endpoints

### 🔐 Authentication
- `POST /api/auth/register` - Đăng ký
- `POST /api/auth/login` - Đăng nhập
- `GET /api/auth/me` - Lấy thông tin user 🔒

### 📝 Rescue Posts
- `GET /api/posts` - Danh sách bài đăng (phân trang)
- `GET /api/posts/{id}` - Chi tiết bài đăng
- `POST /api/posts` - Tạo bài đăng mới 🔒
- `DELETE /api/posts/{id}` - Xóa bài đăng 🔒
- `GET /api/posts/my-posts` - Bài đăng của tôi 🔒

### 👤 User Profile
- `GET /api/users/profile` - Xem profile 🔒
- `PUT /api/users/profile` - Cập nhật profile 🔒
- `POST /api/users/avatar` - Upload avatar 🔒

### 🔔 Notifications
- `GET /api/notifications` - Danh sách thông báo 🔒
- `PUT /api/notifications/{id}/read` - Đánh dấu đã đọc 🔒
- `PUT /api/notifications/read-all` - Đánh dấu tất cả 🔒

### 🤝 Volunteers
- `POST /api/volunteers/offer` - Đăng ký giúp cứu hộ 🔒
- `GET /api/volunteers/my-offers` - Danh sách đã đăng ký 🔒

### 📊 Dashboard
- `GET /api/dashboard/stats` - Thống kê trang chủ

### 👥 Team
- `GET /api/team` - Danh sách đội ngũ

🔒 = Cần JWT token

---

## 🧪 Test API ngay bây giờ!

### Cách 1: Dùng Swagger UI (Dễ nhất)

1. Mở trình duyệt: http://localhost:5125/api/docs
2. Thử API **Register**:
   - Click `POST /api/auth/register`
   - Click "Try it out"
   - Nhập data:
     ```json
     {
       "fullName": "Nguyễn Test",
       "email": "test@example.com",
       "phone": "0123456789",
       "password": "123456",
       "confirmPassword": "123456"
     }
     ```
   - Click "Execute"
   - Copy **token** từ response

3. Thử API cần auth:
   - Click nút **"Authorize"** (góc trên bên phải)
   - Nhập: `Bearer {token_vừa_copy}`
   - Click "Authorize"
   - Giờ bạn có thể test các API có 🔒

### Cách 2: Dùng curl

**Register:**
```bash
curl -X POST http://localhost:5125/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "fullName": "Nguyễn Test",
    "email": "test@example.com",
    "phone": "0123456789",
    "password": "123456",
    "confirmPassword": "123456"
  }'
```

**Login:**
```bash
curl -X POST http://localhost:5125/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "123456"
  }'
```

**Get Posts:**
```bash
curl http://localhost:5125/api/posts?page=1&limit=10
```

---

## 📱 Kết nối với Android App

### 1. Thay đổi Base URL trong Android

Trong file Android của bạn (ví dụ: `ApiClient.java`), đổi BASE_URL:

```java
// Nếu test trên emulator
private static final String BASE_URL = "http://10.0.2.2:5125/api/";

// Nếu test trên thiết bị thật (cùng wifi)
private static final String BASE_URL = "http://192.168.1.XXX:5125/api/";
// (Thay XXX bằng IP máy tính của bạn)
```

**Lấy IP máy tính:**
```cmd
ipconfig
# Tìm "IPv4 Address" của adapter đang dùng
```

### 2. Thêm Internet Permission (AndroidManifest.xml)
```xml
<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />

<application
    android:usesCleartextTraffic="true"
    ...>
```

### 3. Test Login từ Android
```java
// LoginActivity.java
ApiClient.getInstance().login(email, password, new Callback<ApiResponse<AuthResponse>>() {
    @Override
    public void onResponse(Call call, Response response) {
        if (response.isSuccessful()) {
            AuthResponse auth = response.body().getData();
            String token = auth.getToken();
            // Lưu token vào SharedPreferences
            saveToken(token);
            // Chuyển sang MainActivity
            startActivity(new Intent(LoginActivity.this, MainActivity.class));
        }
    }
    
    @Override
    public void onFailure(Call call, Throwable t) {
        Toast.makeText(LoginActivity.this, "Lỗi kết nối", Toast.LENGTH_SHORT).show();
    }
});
```

---

## 📂 Files quan trọng

- **`API_DOCUMENTATION.md`** - Tài liệu đầy đủ tất cả API endpoints
- **`API_README.md`** - Hướng dẫn chi tiết về backend
- **`appsettings.json`** - Configuration (JWT, Database)
- **`Program.cs`** - Entry point, cấu hình services
- **`Controllers/Api/`** - API Controllers
- **`Services/`** - Business logic (JWT, Password, FileUpload)
- **`DTOs/`** - Data Transfer Objects

---

## 🗄️ Database

Database đã được tạo tự động: **pawhelp_db**

Để xem data, dùng **SQL Server Management Studio** hoặc **Azure Data Studio**:
```
Server: .
Database: pawhelp_db
Authentication: Windows Authentication
```

### Seed Data (Optional)

Nếu muốn thêm dữ liệu mẫu (AnimalTypes, TeamMembers), có thể chạy script SQL hoặc tạo qua Web Admin.

---

## 🔧 Troubleshooting

### API không chạy?
```bash
cd c:\doAn\BE_Paw-help
dotnet run
```

### Android không kết nối được?
1. Kiểm tra BASE_URL đúng chưa
2. Kiểm tra `usesCleartextTraffic="true"` trong AndroidManifest
3. Kiểm tra firewall Windows (cho phép port 5125)
4. Ping IP máy tính từ điện thoại

### Lỗi 401 Unauthorized?
- Token có đúng không?
- Token đã hết hạn? (7 ngày)
- Header: `Authorization: Bearer {token}` (có chữ "Bearer ")

---

## 🎓 Học thêm

### Video tutorial tích hợp Retrofit + Android
Tìm trên YouTube: "Retrofit Android Tutorial"

### Tài liệu Retrofit
https://square.github.io/retrofit/

### JWT.io - Debug JWT token
https://jwt.io/ (paste token vào để xem nội dung)

---

## 📞 Cần hỗ trợ?

Nếu gặp vấn đề:
1. Đọc `API_DOCUMENTATION.md` để hiểu rõ API
2. Test qua Swagger UI trước
3. Check console logs của Android
4. Check terminal logs của API

---

## ✨ Next Steps

1. ✅ Test tất cả API qua Swagger UI
2. ✅ Tạo models trong Android (User, Post, Notification...)
3. ✅ Tạo Retrofit API interfaces
4. ✅ Implement Login/Register screens
5. ✅ Implement MainActivity (load posts)
6. ✅ Implement Create Post screen (with image upload)
7. ✅ Implement Profile screen

---

## 🎉 Chúc bạn code thành công!

**"Yêu thương và hành động - cùng nhau tạo nên sự khác biệt!"** 🐾

