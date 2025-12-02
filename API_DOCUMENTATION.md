# 📚 PAW-HELP API DOCUMENTATION

## 🌐 Base URL
```
Development: https://localhost:7XXX/api
Production: https://api.pawhelp.vn/api
```

## 🔐 Authentication
API sử dụng JWT Bearer Token. Sau khi đăng nhập/đăng ký, bạn sẽ nhận được token, thêm vào header của các request tiếp theo:

```
Authorization: Bearer {your_token_here}
```

---

## 📋 API ENDPOINTS

### 🔐 Authentication (`/api/auth`)

#### 1. **POST** `/api/auth/register` - Đăng ký tài khoản mới
**Request Body:**
```json
{
  "fullName": "Nguyễn Văn A",
  "email": "nguyenvana@gmail.com",
  "phone": "0123456789",
  "password": "password123",
  "confirmPassword": "password123"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Đăng ký thành công",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "user": {
      "userId": 1,
      "fullName": "Nguyễn Văn A",
      "email": "nguyenvana@gmail.com",
      "phone": "0123456789",
      "avatarUrl": null,
      "userRole": "user",
      "status": "active",
      "emailVerified": false,
      "createdAt": "2025-12-02T10:30:00Z"
    }
  },
  "errors": null
}
```

---

#### 2. **POST** `/api/auth/login` - Đăng nhập
**Request Body:**
```json
{
  "email": "nguyenvana@gmail.com",
  "password": "password123"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Đăng nhập thành công",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "user": {
      "userId": 1,
      "fullName": "Nguyễn Văn A",
      "email": "nguyenvana@gmail.com",
      "phone": "0123456789",
      "avatarUrl": "/uploads/avatars/abc.jpg",
      "userRole": "user",
      "status": "active",
      "emailVerified": true,
      "createdAt": "2025-12-02T10:30:00Z"
    }
  }
}
```

**Error Response (401 Unauthorized):**
```json
{
  "success": false,
  "message": "Email hoặc mật khẩu không đúng",
  "data": null,
  "errors": null
}
```

---

#### 3. **GET** `/api/auth/me` - Lấy thông tin user hiện tại 🔒
**Headers:**
```
Authorization: Bearer {token}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Thành công",
  "data": {
    "userId": 1,
    "fullName": "Nguyễn Văn A",
    "email": "nguyenvana@gmail.com",
    "phone": "0123456789",
    "avatarUrl": "/uploads/avatars/abc.jpg",
    "userRole": "user",
    "status": "active",
    "emailVerified": true,
    "createdAt": "2025-12-02T10:30:00Z"
  }
}
```

---

### 📝 Rescue Posts (`/api/posts`)

#### 4. **GET** `/api/posts` - Lấy danh sách bài đăng (có phân trang)
**Query Parameters:**
- `page` (int, default: 1)
- `limit` (int, default: 10)
- `status` (string, optional): `waiting`, `processing`, `rescued`, `cancelled`
- `urgencyLevel` (string, optional): `low`, `medium`, `high`, `critical`
- `animalTypeId` (int, optional)

**Example:**
```
GET /api/posts?page=1&limit=10&status=waiting&urgencyLevel=high
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Thành công",
  "data": {
    "items": [
      {
        "postId": 45,
        "title": "Chó con bị thương cần cứu gấp",
        "description": "Chó con khoảng 2 tháng tuổi, bị thương ở chân sau...",
        "location": "123 Lê Duẩn, Đà Nẵng",
        "latitude": 16.0544,
        "longitude": 108.2022,
        "imageUrl": "/uploads/posts/abc123.jpg",
        "status": "waiting",
        "urgencyLevel": "high",
        "contactPhone": "0123456789",
        "viewCount": 23,
        "createdAt": "2025-12-02T10:30:00Z",
        "updatedAt": "2025-12-02T10:30:00Z",
        "animalType": {
          "typeId": 1,
          "typeName": "Chó",
          "typeEmoji": "🐕"
        },
        "user": {
          "userId": 1,
          "fullName": "Nguyễn Văn A",
          "avatarUrl": "/uploads/avatars/user1.jpg",
          "phone": "0123456789"
        },
        "commentCount": 5,
        "volunteerCount": 2,
        "images": [
          "/uploads/posts/abc123.jpg",
          "/uploads/posts/def456.jpg"
        ]
      }
    ],
    "pagination": {
      "page": 1,
      "limit": 10,
      "total": 145,
      "totalPages": 15
    }
  }
}
```

---

#### 5. **GET** `/api/posts/{id}` - Lấy chi tiết bài đăng
**Example:**
```
GET /api/posts/45
```

**Response (200 OK):** Giống format item trong `/api/posts`, nhưng chỉ 1 object

---

#### 6. **POST** `/api/posts` - Tạo bài đăng mới 🔒
**Headers:**
```
Authorization: Bearer {token}
Content-Type: multipart/form-data
```

**Form Data:**
```
title: "Chó con bị thương cần cứu gấp"
description: "Chó con khoảng 2 tháng tuổi..."
animalTypeId: 1
location: "123 Lê Duẩn, Đà Nẵng"
latitude: 16.0544
longitude: 108.2022
urgencyLevel: "high"
contactPhone: "0123456789"
images: [File1, File2] // multipart files
```

**Response (201 Created):**
```json
{
  "success": true,
  "message": "Tạo bài đăng thành công",
  "data": {
    "postId": 45,
    "title": "Chó con bị thương cần cứu gấp",
    "status": "waiting",
    ...
  }
}
```

---

#### 7. **DELETE** `/api/posts/{id}` - Xóa bài đăng 🔒
**Headers:**
```
Authorization: Bearer {token}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Xóa bài đăng thành công",
  "data": null
}
```

---

#### 8. **GET** `/api/posts/my-posts` - Lấy bài đăng của tôi 🔒
**Headers:**
```
Authorization: Bearer {token}
```

**Response (200 OK):** Danh sách bài đăng (không phân trang)

---

### 👤 User Profile (`/api/users`)

#### 9. **GET** `/api/users/profile` - Lấy thông tin profile 🔒
**Headers:**
```
Authorization: Bearer {token}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Thành công",
  "data": {
    "userId": 1,
    "fullName": "Nguyễn Văn A",
    "email": "nguyenvana@gmail.com",
    "phone": "0123456789",
    "avatarUrl": "/uploads/avatars/abc.jpg",
    "userRole": "user",
    "status": "active",
    "emailVerified": true,
    "createdAt": "2025-12-02T10:30:00Z"
  }
}
```

---

#### 10. **PUT** `/api/users/profile` - Cập nhật profile 🔒
**Headers:**
```
Authorization: Bearer {token}
Content-Type: application/json
```

**Request Body:**
```json
{
  "fullName": "Nguyễn Văn B",
  "phone": "0987654321",
  "gender": "Nam",
  "address": "456 Trần Phú, Đà Nẵng"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Cập nhật thông tin thành công",
  "data": { /* UserInfo object */ }
}
```

---

#### 11. **POST** `/api/users/avatar` - Upload avatar 🔒
**Headers:**
```
Authorization: Bearer {token}
Content-Type: multipart/form-data
```

**Form Data:**
```
avatar: [File] // image file
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Upload avatar thành công",
  "data": "/uploads/avatars/xyz789.jpg"
}
```

---

### 🔔 Notifications (`/api/notifications`)

#### 12. **GET** `/api/notifications` - Lấy danh sách thông báo 🔒
**Headers:**
```
Authorization: Bearer {token}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Thành công",
  "data": [
    {
      "notificationId": 1,
      "title": "Có người tình nguyện giúp đỡ",
      "message": "Nguyễn Văn B đã đăng ký giúp cứu hộ bài đăng của bạn",
      "type": "volunteer",
      "relatedPostId": 45,
      "icon": "volunteer",
      "isRead": false,
      "createdAt": "2025-12-02T11:00:00Z"
    }
  ]
}
```

---

#### 13. **PUT** `/api/notifications/{id}/read` - Đánh dấu đã đọc 🔒
**Headers:**
```
Authorization: Bearer {token}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Đánh dấu đã đọc thành công",
  "data": null
}
```

---

#### 14. **PUT** `/api/notifications/read-all` - Đánh dấu tất cả đã đọc 🔒
**Headers:**
```
Authorization: Bearer {token}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Đã đánh dấu tất cả đã đọc",
  "data": null
}
```

---

### 🤝 Volunteers (`/api/volunteers`)

#### 15. **POST** `/api/volunteers/offer` - Đăng ký giúp cứu hộ 🔒
**Headers:**
```
Authorization: Bearer {token}
Content-Type: application/json
```

**Request Body:**
```json
{
  "postId": 45,
  "message": "Tôi có thể giúp cứu hộ bé này"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Đăng ký tình nguyện thành công",
  "data": {
    "volunteerId": 10,
    "postId": 45,
    "status": "offered",
    "message": "Tôi có thể giúp cứu hộ bé này",
    "createdAt": "2025-12-02T12:00:00Z"
  }
}
```

---

#### 16. **GET** `/api/volunteers/my-offers` - Lấy danh sách đã đăng ký 🔒
**Headers:**
```
Authorization: Bearer {token}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Thành công",
  "data": [
    {
      "volunteerId": 10,
      "postId": 45,
      "postTitle": "Chó con bị thương cần cứu gấp",
      "postLocation": "123 Lê Duẩn, Đà Nẵng",
      "postStatus": "waiting",
      "postImageUrl": "/uploads/posts/abc123.jpg",
      "animalType": "Chó",
      "volunteerStatus": "offered",
      "createdAt": "2025-12-02T12:00:00Z"
    }
  ]
}
```

---

### 📊 Dashboard (`/api/dashboard`)

#### 17. **GET** `/api/dashboard/stats` - Lấy thống kê trang chủ
**Response (200 OK):**
```json
{
  "success": true,
  "message": "Thành công",
  "data": {
    "sosCount": 12,
    "rescuedCount": 145,
    "totalPosts": 234,
    "activeVolunteers": 56
  }
}
```

---

### 👥 Team (`/api/team`)

#### 18. **GET** `/api/team` - Lấy danh sách đội ngũ
**Response (200 OK):**
```json
{
  "success": true,
  "message": "Thành công",
  "data": [
    {
      "memberId": 1,
      "fullName": "Nguyễn Thị C",
      "role": "Trưởng nhóm cứu hộ",
      "position": "Leader",
      "description": "10 năm kinh nghiệm cứu hộ động vật",
      "avatarUrl": "/uploads/team/member1.jpg",
      "email": "leader@pawhelp.vn",
      "phone": "0123456789",
      "teamName": "Đội Cứu Hộ Khẩn Cấp"
    }
  ]
}
```

---

## 🔑 Status Codes

| Code | Meaning |
|------|---------|
| 200 | OK - Request thành công |
| 201 | Created - Tạo resource thành công |
| 400 | Bad Request - Dữ liệu không hợp lệ |
| 401 | Unauthorized - Chưa đăng nhập hoặc token không hợp lệ |
| 403 | Forbidden - Không có quyền truy cập |
| 404 | Not Found - Không tìm thấy resource |
| 500 | Internal Server Error - Lỗi server |

---

## 🧪 Testing với Swagger UI

Khi chạy project ở Development mode, truy cập:
```
https://localhost:7XXX/api/docs
```

Swagger UI cho phép:
- Xem tất cả endpoints
- Test API trực tiếp
- Thêm JWT token vào header (nút "Authorize")

---

## 📱 Android Integration Example (Retrofit)

### 1. Thêm dependencies vào `build.gradle`:
```gradle
dependencies {
    implementation 'com.squareup.retrofit2:retrofit:2.9.0'
    implementation 'com.squareup.retrofit2:converter-gson:2.9.0'
    implementation 'com.squareup.okhttp3:logging-interceptor:4.11.0'
}
```

### 2. Tạo API Service Interface:
```java
public interface PawHelpApi {
    @POST("auth/login")
    Call<ApiResponse<AuthResponse>> login(@Body LoginRequest request);
    
    @GET("posts")
    Call<ApiResponse<PaginatedResponse<PostResponse>>> getPosts(
        @Query("page") int page,
        @Query("limit") int limit
    );
    
    @Multipart
    @POST("posts")
    Call<ApiResponse<PostResponse>> createPost(
        @Part("title") RequestBody title,
        @Part("description") RequestBody description,
        @Part("location") RequestBody location,
        @Part MultipartBody.Part image
    );
}
```

### 3. Setup Retrofit Client:
```java
OkHttpClient client = new OkHttpClient.Builder()
    .addInterceptor(chain -> {
        Request original = chain.request();
        Request.Builder requestBuilder = original.newBuilder()
            .header("Authorization", "Bearer " + token);
        return chain.proceed(requestBuilder.build());
    })
    .build();

Retrofit retrofit = new Retrofit.Builder()
    .baseUrl("https://your-api-url/api/")
    .client(client)
    .addConverterFactory(GsonConverterFactory.create())
    .build();

PawHelpApi api = retrofit.create(PawHelpApi.class);
```

---

## 🎯 Best Practices

1. **Lưu token trong SharedPreferences (Android)**
2. **Refresh token khi hết hạn** (hiện tại token hết hạn sau 7 ngày)
3. **Xử lý lỗi network gracefully**
4. **Show loading indicator khi gọi API**
5. **Cache data locally** (Room Database)

---

## 📞 Support

Nếu có vấn đề, liên hệ:
- Email: contact@pawhelp.vn
- Hotline: 113

