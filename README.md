# PawHelp - Hệ thống quản lý cứu hộ động vật

Dự án ASP.NET Core MVC chuyên dụng cho quản trị viên hệ thống cứu hộ động vật.

## Tính năng

### Đăng nhập
- Trang đăng nhập đơn giản với giao diện đẹp
- Tài khoản demo: 
  - **admin / admin123**
  - **staff / staff123**
- Quản lý session
- Nút đăng xuất

### Trang quản trị
- **Dashboard**: Tổng quan thống kê hệ thống
  - Thống kê số lượng động vật
  - Thống kê yêu cầu cứu hộ
  - Danh sách hoạt động gần đây

- **Quản lý động vật**: 
  - Xem danh sách động vật đã cứu hộ
  - Thêm động vật mới
  - Chỉnh sửa thông tin động vật
  - Xóa động vật
  - Xem chi tiết động vật (thông tin cơ bản, sức khỏe, lịch sử)
  - Theo dõi trạng thái: Đang cứu hộ, Đã điều trị, Sẵn sàng nhận nuôi, Đã được nhận nuôi

- **Quản lý yêu cầu cứu hộ**:
  - Xem danh sách yêu cầu cứu hộ
  - Xem chi tiết yêu cầu
  - Cập nhật trạng thái: Chờ xử lý, Đang xử lý, Hoàn thành, Hủy bỏ
  - Phân công nhiệm vụ cho nhân viên
  - Thêm ghi chú quản lý
  - Phân loại độ ưu tiên: Khẩn cấp, Cao, Trung bình, Thấp

- **Quản lý người dùng**:
  - Xem danh sách người dùng
  - Thêm người dùng mới
  - Chỉnh sửa thông tin người dùng
  - Kích hoạt/Vô hiệu hóa tài khoản
  - Phân quyền: Admin, Staff, Volunteer

## Cấu trúc dự án

```
PawHelp/
├── Controllers/
│   ├── AuthController.cs          # Controller đăng nhập/đăng xuất
│   └── AdminController.cs         # Controller trang quản trị
├── Models/
│   ├── Animal.cs                  # Model động vật
│   ├── RescueRequest.cs           # Model yêu cầu cứu hộ
│   ├── User.cs                    # Model người dùng
│   └── ErrorViewModel.cs          # Model xử lý lỗi
├── Views/
│   ├── Auth/                      # Views đăng nhập
│   │   └── Login.cshtml          # Trang đăng nhập
│   ├── Admin/                     # Views trang quản trị
│   │   ├── Index.cshtml          # Dashboard
│   │   ├── Animals.cshtml        # Danh sách động vật
│   │   ├── AnimalDetails.cshtml  # Chi tiết động vật
│   │   ├── CreateAnimal.cshtml   # Thêm động vật
│   │   ├── EditAnimal.cshtml     # Sửa động vật
│   │   ├── RescueRequests.cshtml # Danh sách yêu cầu
│   │   ├── RequestDetails.cshtml # Chi tiết yêu cầu
│   │   ├── Users.cshtml          # Danh sách người dùng
│   │   ├── UserDetails.cshtml    # Chi tiết người dùng
│   │   ├── CreateUser.cshtml     # Thêm người dùng
│   │   └── EditUser.cshtml       # Sửa người dùng
│   └── Shared/
│       ├── _Layout.cshtml        # Layout chính
│       └── _AdminLayout.cshtml   # Layout admin
├── wwwroot/                       # Static files (CSS, JS, images)
├── Program.cs                     # Entry point
├── appsettings.json              # Cấu hình ứng dụng
└── PawHelp.csproj                # File dự án

```

## Công nghệ sử dụng

- **Framework**: ASP.NET Core 9.0 MVC
- **UI Framework**: Bootstrap 5
- **Icons**: Emoji (đơn giản, dễ sử dụng)
- **Validation**: jQuery Validation

## Cài đặt và chạy dự án

### Yêu cầu
- .NET SDK 9.0 hoặc mới hơn
- Visual Studio 2022 / VS Code / Rider

### Các bước chạy

1. Clone hoặc tải dự án về máy

2. Mở terminal tại thư mục dự án

3. Restore dependencies:
```bash
dotnet restore
```

4. Chạy ứng dụng:
```bash
dotnet run
```

5. Mở trình duyệt và truy cập:
```
https://localhost:5001
hoặc
http://localhost:5000
```

6. Đăng nhập với tài khoản demo:
   - Username: `admin`
   - Password: `admin123`

## URL Routes

### Authentication
- `/Auth/Login` - Trang đăng nhập
- `/Auth/Logout` - Đăng xuất

### Quản trị (Admin)
- `/Admin` - Dashboard
- `/Admin/Animals` - Quản lý động vật
- `/Admin/Animals/Create` - Thêm động vật
- `/Admin/Animals/Edit/{id}` - Sửa động vật
- `/Admin/Animals/Details/{id}` - Chi tiết động vật
- `/Admin/RescueRequests` - Danh sách yêu cầu cứu hộ
- `/Admin/RescueRequests/Details/{id}` - Chi tiết yêu cầu
- `/Admin/Users` - Quản lý người dùng
- `/Admin/Users/Create` - Thêm người dùng
- `/Admin/Users/Edit/{id}` - Sửa người dùng
- `/Admin/Users/Details/{id}` - Chi tiết người dùng

## Lưu ý

- **Chỉ dành cho Admin**: Hệ thống này chỉ dành cho quản trị viên, không có phần trang công khai.
- **Đăng nhập đơn giản**: Sử dụng Session, chưa có mã hóa mật khẩu. Trong production cần dùng ASP.NET Core Identity.
- **Dữ liệu tạm thời**: Hiện tại dự án sử dụng dữ liệu trong bộ nhớ (static lists). Dữ liệu sẽ bị mất khi restart ứng dụng.
- **Tích hợp Database**: Để sử dụng lâu dài, cần tích hợp Entity Framework Core với SQL Server/PostgreSQL/MySQL.
- **File Upload**: Hiện tại chỉ nhập URL hình ảnh. Có thể bổ sung tính năng upload file.

## Phát triển tiếp

### Tính năng cần bổ sung:
1. Tích hợp Database (Entity Framework Core)
2. Authentication nâng cao (ASP.NET Core Identity, mã hóa password)
3. Authorization (phân quyền theo Role)
4. Upload hình ảnh
4. Xuất báo cáo (PDF, Excel)
5. Gửi email/SMS thông báo
6. API RESTful cho mobile app
7. Tích hợp bản đồ (Google Maps) cho địa điểm cứu hộ
8. Dashboard với biểu đồ thống kê
9. Lịch sử hoạt động (Activity Log)
10. Tính năng tìm kiếm và lọc nâng cao

## Tác giả

Dự án được phát triển cho hệ thống quản lý cứu hộ động vật PawHelp.

## License

MIT License

"# Be_Paw-Help" 
