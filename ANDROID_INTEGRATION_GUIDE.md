# 📱 HƯỚNG DẪN TÍCH HỢP ANDROID VỚI PAW-HELP API

## 🎯 Mục tiêu

Kết nối app Android **Paw-help** với backend API đã được cấu hình sẵn.

---

## 📋 CHECKLIST

### Backend (✅ Đã xong)
- ✅ API đã chạy tại `http://localhost:5125/api`
- ✅ Swagger UI: `http://localhost:5125/api/docs`
- ✅ 18 endpoints sẵn sàng
- ✅ JWT authentication
- ✅ File upload support

### Android (🔲 Cần làm)
- 🔲 Thêm Retrofit dependencies
- 🔲 Tạo Models (POJOs)
- 🔲 Tạo API Service interfaces
- 🔲 Setup RetrofitClient
- 🔲 Implement Authentication
- 🔲 Implement API calls

---

## 🚀 BƯỚC 1: THÊM DEPENDENCIES

### File: `app/build.gradle`

```gradle
dependencies {
    // Existing dependencies...
    
    // Retrofit cho API calls
    implementation 'com.squareup.retrofit2:retrofit:2.9.0'
    implementation 'com.squareup.retrofit2:converter-gson:2.9.0'
    
    // OkHttp cho logging (optional, debug purposes)
    implementation 'com.squareup.okhttp3:logging-interceptor:4.11.0'
    
    // Gson cho JSON parsing
    implementation 'com.google.code.gson:gson:2.10.1'
}
```

Sau đó: **Sync Gradle**

---

## 🚀 BƯỚC 2: THÊM INTERNET PERMISSION

### File: `AndroidManifest.xml`

```xml
<?xml version="1.0" encoding="utf-8"?>
<manifest xmlns:android="http://schemas.android.com/apk/res/android">

    <!-- Thêm 2 dòng này -->
    <uses-permission android:name="android.permission.INTERNET" />
    <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />

    <application
        android:usesCleartextTraffic="true"
        android:name=".MyApplication"
        android:allowBackup="true"
        ...>
        
        <!-- Activities -->
        
    </application>
</manifest>
```

**Quan trọng:** `android:usesCleartextTraffic="true"` cho phép HTTP (development)

---

## 🚀 BƯỚC 3: TẠO MODELS (POJOs)

### File: `com/example/paw_help/models/ApiResponse.java`

```java
package com.example.paw_help.models;

public class ApiResponse<T> {
    private boolean success;
    private String message;
    private T data;
    private java.util.List<String> errors;

    // Getters and Setters
    public boolean isSuccess() { return success; }
    public void setSuccess(boolean success) { this.success = success; }
    
    public String getMessage() { return message; }
    public void setMessage(String message) { this.message = message; }
    
    public T getData() { return data; }
    public void setData(T data) { this.data = data; }
    
    public java.util.List<String> getErrors() { return errors; }
    public void setErrors(java.util.List<String> errors) { this.errors = errors; }
}
```

### File: `com/example/paw_help/models/User.java`

```java
package com.example.paw_help.models;

public class User {
    private int userId;
    private String fullName;
    private String email;
    private String phone;
    private String avatarUrl;
    private String userRole;
    private String status;
    private boolean emailVerified;
    private String createdAt;

    // Getters and Setters
    public int getUserId() { return userId; }
    public void setUserId(int userId) { this.userId = userId; }
    
    public String getFullName() { return fullName; }
    public void setFullName(String fullName) { this.fullName = fullName; }
    
    public String getEmail() { return email; }
    public void setEmail(String email) { this.email = email; }
    
    public String getPhone() { return phone; }
    public void setPhone(String phone) { this.phone = phone; }
    
    public String getAvatarUrl() { return avatarUrl; }
    public void setAvatarUrl(String avatarUrl) { this.avatarUrl = avatarUrl; }
    
    public String getUserRole() { return userRole; }
    public void setUserRole(String userRole) { this.userRole = userRole; }
    
    // ... other getters/setters
}
```

### File: `com/example/paw_help/models/AuthResponse.java`

```java
package com.example.paw_help.models;

public class AuthResponse {
    private String token;
    private User user;

    public String getToken() { return token; }
    public void setToken(String token) { this.token = token; }
    
    public User getUser() { return user; }
    public void setUser(User user) { this.user = user; }
}
```

### File: `com/example/paw_help/models/LoginRequest.java`

```java
package com.example.paw_help.models;

public class LoginRequest {
    private String email;
    private String password;

    public LoginRequest(String email, String password) {
        this.email = email;
        this.password = password;
    }

    public String getEmail() { return email; }
    public void setEmail(String email) { this.email = email; }
    
    public String getPassword() { return password; }
    public void setPassword(String password) { this.password = password; }
}
```

### File: `com/example/paw_help/models/RegisterRequest.java`

```java
package com.example.paw_help.models;

public class RegisterRequest {
    private String fullName;
    private String email;
    private String phone;
    private String password;
    private String confirmPassword;

    public RegisterRequest(String fullName, String email, String phone, String password, String confirmPassword) {
        this.fullName = fullName;
        this.email = email;
        this.phone = phone;
        this.password = password;
        this.confirmPassword = confirmPassword;
    }

    // Getters and Setters
    public String getFullName() { return fullName; }
    public void setFullName(String fullName) { this.fullName = fullName; }
    
    public String getEmail() { return email; }
    public void setEmail(String email) { this.email = email; }
    
    public String getPhone() { return phone; }
    public void setPhone(String phone) { this.phone = phone; }
    
    public String getPassword() { return password; }
    public void setPassword(String password) { this.password = password; }
    
    public String getConfirmPassword() { return confirmPassword; }
    public void setConfirmPassword(String confirmPassword) { this.confirmPassword = confirmPassword; }
}
```

### File: `com/example/paw_help/models/Post.java`

```java
package com.example.paw_help.models;

import java.util.List;

public class Post {
    private int postId;
    private String title;
    private String description;
    private String location;
    private double latitude;
    private double longitude;
    private String imageUrl;
    private String status;
    private String urgencyLevel;
    private String contactPhone;
    private int viewCount;
    private String createdAt;
    private String updatedAt;
    private AnimalType animalType;
    private User user;
    private int commentCount;
    private int volunteerCount;
    private List<String> images;

    // Getters and Setters
    // ... (tự generate trong IDE)
}

class AnimalType {
    private int typeId;
    private String typeName;
    private String typeEmoji;
    
    // Getters and Setters
}
```

---

## 🚀 BƯỚC 4: TẠO API SERVICE INTERFACE

### File: `com/example/paw_help/api/PawHelpApi.java`

```java
package com.example.paw_help.api;

import com.example.paw_help.models.*;
import okhttp3.MultipartBody;
import okhttp3.RequestBody;
import retrofit2.Call;
import retrofit2.http.*;

import java.util.List;

public interface PawHelpApi {
    
    // ==================== AUTH ====================
    
    @POST("auth/register")
    Call<ApiResponse<AuthResponse>> register(@Body RegisterRequest request);
    
    @POST("auth/login")
    Call<ApiResponse<AuthResponse>> login(@Body LoginRequest request);
    
    @GET("auth/me")
    Call<ApiResponse<User>> getCurrentUser();
    
    // ==================== POSTS ====================
    
    @GET("posts")
    Call<ApiResponse<PostListResponse>> getPosts(
        @Query("page") int page,
        @Query("limit") int limit,
        @Query("status") String status,
        @Query("urgencyLevel") String urgencyLevel
    );
    
    @GET("posts/{id}")
    Call<ApiResponse<Post>> getPost(@Path("id") int id);
    
    @Multipart
    @POST("posts")
    Call<ApiResponse<Post>> createPost(
        @Part("title") RequestBody title,
        @Part("description") RequestBody description,
        @Part("animalTypeId") RequestBody animalTypeId,
        @Part("location") RequestBody location,
        @Part("latitude") RequestBody latitude,
        @Part("longitude") RequestBody longitude,
        @Part("urgencyLevel") RequestBody urgencyLevel,
        @Part("contactPhone") RequestBody contactPhone,
        @Part List<MultipartBody.Part> images
    );
    
    @DELETE("posts/{id}")
    Call<ApiResponse<Object>> deletePost(@Path("id") int id);
    
    @GET("posts/my-posts")
    Call<ApiResponse<List<Post>>> getMyPosts();
    
    // ==================== USER ====================
    
    @GET("users/profile")
    Call<ApiResponse<User>> getProfile();
    
    @PUT("users/profile")
    Call<ApiResponse<User>> updateProfile(@Body UpdateProfileRequest request);
    
    @Multipart
    @POST("users/avatar")
    Call<ApiResponse<String>> uploadAvatar(@Part MultipartBody.Part avatar);
    
    // ==================== NOTIFICATIONS ====================
    
    @GET("notifications")
    Call<ApiResponse<List<Notification>>> getNotifications();
    
    @PUT("notifications/{id}/read")
    Call<ApiResponse<Object>> markNotificationAsRead(@Path("id") int id);
    
    @PUT("notifications/read-all")
    Call<ApiResponse<Object>> markAllNotificationsAsRead();
    
    // ==================== VOLUNTEERS ====================
    
    @POST("volunteers/offer")
    Call<ApiResponse<VolunteerResponse>> offerHelp(@Body OfferHelpRequest request);
    
    @GET("volunteers/my-offers")
    Call<ApiResponse<List<MyVolunteerOffer>>> getMyOffers();
    
    // ==================== DASHBOARD ====================
    
    @GET("dashboard/stats")
    Call<ApiResponse<DashboardStats>> getDashboardStats();
    
    // ==================== TEAM ====================
    
    @GET("team")
    Call<ApiResponse<List<TeamMember>>> getTeamMembers();
}
```

---

## 🚀 BƯỚC 5: SETUP RETROFIT CLIENT

### File: `com/example/paw_help/api/RetrofitClient.java`

```java
package com.example.paw_help.api;

import android.content.Context;
import android.content.SharedPreferences;

import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.logging.HttpLoggingInterceptor;
import retrofit2.Retrofit;
import retrofit2.converter.gson.GsonConverterFactory;

import java.util.concurrent.TimeUnit;

public class RetrofitClient {
    
    // ⚠️ ĐỔI BASE_URL phù hợp với môi trường của bạn
    private static final String BASE_URL = "http://10.0.2.2:5125/api/"; // Emulator
    // private static final String BASE_URL = "http://192.168.1.XXX:5125/api/"; // Thiết bị thật
    
    private static RetrofitClient instance;
    private Retrofit retrofit;
    private PawHelpApi api;
    private Context context;
    
    private RetrofitClient(Context context) {
        this.context = context.getApplicationContext();
        
        // Logging interceptor (debug purposes)
        HttpLoggingInterceptor loggingInterceptor = new HttpLoggingInterceptor();
        loggingInterceptor.setLevel(HttpLoggingInterceptor.Level.BODY);
        
        // OkHttp client với JWT token interceptor
        OkHttpClient okHttpClient = new OkHttpClient.Builder()
            .addInterceptor(loggingInterceptor)
            .addInterceptor(chain -> {
                Request originalRequest = chain.request();
                
                // Lấy token từ SharedPreferences
                String token = getToken();
                
                if (token != null && !token.isEmpty()) {
                    // Thêm Authorization header
                    Request newRequest = originalRequest.newBuilder()
                        .header("Authorization", "Bearer " + token)
                        .build();
                    return chain.proceed(newRequest);
                }
                
                return chain.proceed(originalRequest);
            })
            .connectTimeout(30, TimeUnit.SECONDS)
            .readTimeout(30, TimeUnit.SECONDS)
            .writeTimeout(30, TimeUnit.SECONDS)
            .build();
        
        // Retrofit instance
        retrofit = new Retrofit.Builder()
            .baseUrl(BASE_URL)
            .client(okHttpClient)
            .addConverterFactory(GsonConverterFactory.create())
            .build();
        
        api = retrofit.create(PawHelpApi.class);
    }
    
    public static synchronized RetrofitClient getInstance(Context context) {
        if (instance == null) {
            instance = new RetrofitClient(context);
        }
        return instance;
    }
    
    public PawHelpApi getApi() {
        return api;
    }
    
    // ==================== TOKEN MANAGEMENT ====================
    
    public void saveToken(String token) {
        SharedPreferences prefs = context.getSharedPreferences("PawHelp", Context.MODE_PRIVATE);
        prefs.edit().putString("jwt_token", token).apply();
    }
    
    public String getToken() {
        SharedPreferences prefs = context.getSharedPreferences("PawHelp", Context.MODE_PRIVATE);
        return prefs.getString("jwt_token", null);
    }
    
    public void clearToken() {
        SharedPreferences prefs = context.getSharedPreferences("PawHelp", Context.MODE_PRIVATE);
        prefs.edit().remove("jwt_token").apply();
    }
    
    public boolean isLoggedIn() {
        String token = getToken();
        return token != null && !token.isEmpty();
    }
    
    // ==================== USER INFO ====================
    
    public void saveUser(User user) {
        SharedPreferences prefs = context.getSharedPreferences("PawHelp", Context.MODE_PRIVATE);
        prefs.edit()
            .putInt("user_id", user.getUserId())
            .putString("user_name", user.getFullName())
            .putString("user_email", user.getEmail())
            .putString("user_avatar", user.getAvatarUrl())
            .apply();
    }
    
    public void clearUser() {
        SharedPreferences prefs = context.getSharedPreferences("PawHelp", Context.MODE_PRIVATE);
        prefs.edit()
            .remove("user_id")
            .remove("user_name")
            .remove("user_email")
            .remove("user_avatar")
            .apply();
    }
    
    public void logout() {
        clearToken();
        clearUser();
    }
}
```

---

## 🚀 BƯỚC 6: SỬ DỤNG API TRONG ACTIVITIES

### Example: `LoginActivity.java`

```java
package com.example.paw_help;

import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ProgressBar;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;

import com.example.paw_help.api.RetrofitClient;
import com.example.paw_help.models.*;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class LoginActivity extends AppCompatActivity {
    
    private EditText etEmail, etPassword;
    private Button btnLogin;
    private ProgressBar progressBar;
    private RetrofitClient retrofitClient;
    
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_login);
        
        // Initialize views
        etEmail = findViewById(R.id.etEmail);
        etPassword = findViewById(R.id.etPassword);
        btnLogin = findViewById(R.id.btnLogin);
        progressBar = findViewById(R.id.progressBar);
        
        // Initialize Retrofit
        retrofitClient = RetrofitClient.getInstance(this);
        
        // Check if already logged in
        if (retrofitClient.isLoggedIn()) {
            goToMainActivity();
            return;
        }
        
        // Login button click
        btnLogin.setOnClickListener(v -> login());
    }
    
    private void login() {
        String email = etEmail.getText().toString().trim();
        String password = etPassword.getText().toString().trim();
        
        // Validation
        if (email.isEmpty()) {
            etEmail.setError("Email là bắt buộc");
            return;
        }
        if (password.isEmpty()) {
            etPassword.setError("Mật khẩu là bắt buộc");
            return;
        }
        
        // Show loading
        setLoading(true);
        
        // Call API
        LoginRequest request = new LoginRequest(email, password);
        Call<ApiResponse<AuthResponse>> call = retrofitClient.getApi().login(request);
        
        call.enqueue(new Callback<ApiResponse<AuthResponse>>() {
            @Override
            public void onResponse(Call<ApiResponse<AuthResponse>> call, Response<ApiResponse<AuthResponse>> response) {
                setLoading(false);
                
                if (response.isSuccessful() && response.body() != null) {
                    ApiResponse<AuthResponse> apiResponse = response.body();
                    
                    if (apiResponse.isSuccess()) {
                        AuthResponse authResponse = apiResponse.getData();
                        
                        // Save token & user info
                        retrofitClient.saveToken(authResponse.getToken());
                        retrofitClient.saveUser(authResponse.getUser());
                        
                        // Show success message
                        Toast.makeText(LoginActivity.this, "Đăng nhập thành công!", Toast.LENGTH_SHORT).show();
                        
                        // Go to MainActivity
                        goToMainActivity();
                    } else {
                        Toast.makeText(LoginActivity.this, apiResponse.getMessage(), Toast.LENGTH_SHORT).show();
                    }
                } else {
                    Toast.makeText(LoginActivity.this, "Đăng nhập thất bại", Toast.LENGTH_SHORT).show();
                }
            }
            
            @Override
            public void onFailure(Call<ApiResponse<AuthResponse>> call, Throwable t) {
                setLoading(false);
                Toast.makeText(LoginActivity.this, "Lỗi kết nối: " + t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }
    
    private void setLoading(boolean isLoading) {
        progressBar.setVisibility(isLoading ? View.VISIBLE : View.GONE);
        btnLogin.setEnabled(!isLoading);
    }
    
    private void goToMainActivity() {
        Intent intent = new Intent(LoginActivity.this, MainActivity.class);
        intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_CLEAR_TASK);
        startActivity(intent);
        finish();
    }
}
```

### Example: `MainActivity.java` - Load Posts

```java
private void loadPosts() {
    RetrofitClient retrofitClient = RetrofitClient.getInstance(this);
    Call<ApiResponse<PostListResponse>> call = retrofitClient.getApi().getPosts(1, 10, null, null);
    
    call.enqueue(new Callback<ApiResponse<PostListResponse>>() {
        @Override
        public void onResponse(Call<ApiResponse<PostListResponse>> call, Response<ApiResponse<PostListResponse>> response) {
            if (response.isSuccessful() && response.body() != null) {
                ApiResponse<PostListResponse> apiResponse = response.body();
                
                if (apiResponse.isSuccess()) {
                    List<Post> posts = apiResponse.getData().getItems();
                    // Update RecyclerView with posts
                    adapter.setPosts(posts);
                }
            }
        }
        
        @Override
        public void onFailure(Call<ApiResponse<PostListResponse>> call, Throwable t) {
            Toast.makeText(MainActivity.this, "Lỗi tải dữ liệu", Toast.LENGTH_SHORT).show();
        }
    });
}
```

---

## 🎯 TESTING

### 1. Test Login
- Chạy API: `dotnet run` trong `BE_Paw-help`
- Chạy Android app
- Thử đăng nhập với tài khoản đã tạo
- Kiểm tra logcat xem có token không

### 2. Test Load Posts
- Sau khi login thành công
- Vào MainActivity
- Kiểm tra xem posts có load không

### 3. Debug Tips
```java
// Trong RetrofitClient, đã có HttpLoggingInterceptor
// Xem logs trong Logcat filter "OkHttp"
```

---

## 📞 TROUBLESHOOTING

### Lỗi: "Unable to resolve host"
**Fix:** Kiểm tra BASE_URL đúng chưa
- Emulator: `http://10.0.2.2:5125/api/`
- Thiết bị: `http://192.168.X.X:5125/api/`

### Lỗi: "Cleartext traffic not permitted"
**Fix:** Thêm `android:usesCleartextTraffic="true"` vào AndroidManifest

### Lỗi: 401 Unauthorized
**Fix:** Token không hợp lệ hoặc hết hạn
- Clear app data
- Login lại

---

## ✅ DONE!

Bạn đã sẵn sàng tích hợp Android với Backend API! 🎉

**Next:** Implement từng màn hình một, test kỹ từng API call.

