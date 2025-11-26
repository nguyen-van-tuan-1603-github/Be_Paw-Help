-- =====================================================
-- PAW HELP - CSDL SQL SERVER
-- Hệ thống cứu hộ động vật Đà Nẵng
-- Version: 1.0
-- Date: 25/11/2025
-- =====================================================

-- Tạo database
USE master;
GO

IF EXISTS(SELECT 1 FROM sys.databases WHERE name = 'pawhelp_db')
BEGIN
    ALTER DATABASE pawhelp_db SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE pawhelp_db;
END
GO

CREATE DATABASE pawhelp_db;
GO

USE pawhelp_db;
GO

-- =====================================================
-- 1. BẢNG USERS - Người dùng
-- =====================================================
CREATE TABLE users (
    user_id INT IDENTITY(1,1) PRIMARY KEY,
    full_name NVARCHAR(100) NOT NULL,
    email NVARCHAR(100) UNIQUE NOT NULL,
    phone NVARCHAR(20),
    password_hash NVARCHAR(255) NOT NULL,
    avatar_url NVARCHAR(255),
    user_role NVARCHAR(20) CHECK (user_role IN ('user', 'volunteer', 'admin')) DEFAULT 'user',
    status NVARCHAR(20) CHECK (status IN ('active', 'inactive', 'banned')) DEFAULT 'active',
    email_verified BIT DEFAULT 0,
    created_at DATETIME2 DEFAULT GETDATE(),
    updated_at DATETIME2 DEFAULT GETDATE(),
    last_login DATETIME2 NULL
);
GO

-- =====================================================
-- 2. BẢNG ANIMAL_TYPES - Loại động vật
-- =====================================================
CREATE TABLE animal_types (
    type_id INT IDENTITY(1,1) PRIMARY KEY,
    type_name NVARCHAR(50) NOT NULL,
    type_emoji NVARCHAR(10),
    description NVARCHAR(MAX),
    created_at DATETIME2 DEFAULT GETDATE()
);
GO

-- =====================================================
-- 3. BẢNG RESCUE_POSTS - Bài đăng cứu hộ
-- =====================================================
CREATE TABLE rescue_posts (
    post_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    title NVARCHAR(255) NOT NULL,
    description NVARCHAR(MAX) NOT NULL,
    animal_type_id INT,
    location NVARCHAR(255) NOT NULL,
    latitude DECIMAL(10, 8),
    longitude DECIMAL(11, 8),
    image_url NVARCHAR(255),
    status NVARCHAR(20) CHECK (status IN ('waiting', 'processing', 'rescued', 'cancelled')) DEFAULT 'waiting',
    urgency_level NVARCHAR(20) CHECK (urgency_level IN ('low', 'medium', 'high', 'critical')) DEFAULT 'medium',
    contact_phone NVARCHAR(20),
    view_count INT DEFAULT 0,
    created_at DATETIME2 DEFAULT GETDATE(),
    updated_at DATETIME2 DEFAULT GETDATE(),
    rescued_at DATETIME2 NULL,
    
    FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE,
    FOREIGN KEY (animal_type_id) REFERENCES animal_types(type_id) ON DELETE SET NULL
);
GO

-- =====================================================
-- 4. BẢNG RESCUE_VOLUNTEERS - Tình nguyện viên cứu hộ
-- =====================================================
CREATE TABLE rescue_volunteers (
    volunteer_id INT IDENTITY(1,1) PRIMARY KEY,
    post_id INT NOT NULL,
    user_id INT NOT NULL,
    status NVARCHAR(20) CHECK (status IN ('offered', 'accepted', 'declined', 'completed')) DEFAULT 'offered',
    message NVARCHAR(MAX),
    created_at DATETIME2 DEFAULT GETDATE(),
    updated_at DATETIME2 DEFAULT GETDATE(),
    
    FOREIGN KEY (post_id) REFERENCES rescue_posts(post_id) ON DELETE CASCADE,
    FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE NO ACTION,
    
    CONSTRAINT unique_volunteer UNIQUE (post_id, user_id)
);
GO

-- =====================================================
-- 5. BẢNG COMMENTS - Bình luận
-- =====================================================
CREATE TABLE comments (
    comment_id INT IDENTITY(1,1) PRIMARY KEY,
    post_id INT NOT NULL,
    user_id INT NOT NULL,
    content NVARCHAR(MAX) NOT NULL,
    parent_comment_id INT NULL,
    created_at DATETIME2 DEFAULT GETDATE(),
    updated_at DATETIME2 DEFAULT GETDATE(),
    
    FOREIGN KEY (post_id) REFERENCES rescue_posts(post_id) ON DELETE CASCADE,
    FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE NO ACTION,
    FOREIGN KEY (parent_comment_id) REFERENCES comments(comment_id) ON DELETE NO ACTION
);
GO

-- =====================================================
-- 6. BẢNG NOTIFICATIONS - Thông báo
-- =====================================================
CREATE TABLE notifications (
    notification_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    title NVARCHAR(255) NOT NULL,
    message NVARCHAR(MAX) NOT NULL,
    type NVARCHAR(20) CHECK (type IN ('rescue', 'comment', 'volunteer', 'system')) DEFAULT 'system',
    related_post_id INT NULL,
    icon NVARCHAR(50),
    is_read BIT DEFAULT 0,
    created_at DATETIME2 DEFAULT GETDATE(),
    
    FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE,
    FOREIGN KEY (related_post_id) REFERENCES rescue_posts(post_id) ON DELETE NO ACTION
);
GO

-- =====================================================
-- 7. BẢNG REPORTS - Báo cáo bài đăng
-- =====================================================
CREATE TABLE reports (
    report_id INT IDENTITY(1,1) PRIMARY KEY,
    post_id INT NOT NULL,
    user_id INT NOT NULL,
    reason NVARCHAR(20) CHECK (reason IN ('fake', 'inappropriate', 'spam', 'scam', 'other')) NOT NULL,
    description NVARCHAR(MAX),
    status NVARCHAR(20) CHECK (status IN ('pending', 'reviewed', 'resolved', 'rejected')) DEFAULT 'pending',
    created_at DATETIME2 DEFAULT GETDATE(),
    resolved_at DATETIME2 NULL,
    
    FOREIGN KEY (post_id) REFERENCES rescue_posts(post_id) ON DELETE CASCADE,
    FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE NO ACTION
);
GO

-- =====================================================
-- 8. BẢNG TEAM_MEMBERS - Thành viên đội ngũ
-- =====================================================
CREATE TABLE team_members (
    member_id INT IDENTITY(1,1) PRIMARY KEY,
    full_name NVARCHAR(100) NOT NULL,
    role NVARCHAR(100) NOT NULL,
    position NVARCHAR(100),
    description NVARCHAR(MAX),
    avatar_url NVARCHAR(255),
    email NVARCHAR(100),
    phone NVARCHAR(20),
    team_name NVARCHAR(100),
    display_order INT DEFAULT 0,
    is_active BIT DEFAULT 1,
    created_at DATETIME2 DEFAULT GETDATE()
);
GO

-- =====================================================
-- 9. BẢNG DONATIONS - Ủng hộ
-- =====================================================
CREATE TABLE donations (
    donation_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NULL,
    donor_name NVARCHAR(100),
    donor_email NVARCHAR(100),
    donor_phone NVARCHAR(20),
    amount DECIMAL(10, 2) NOT NULL,
    message NVARCHAR(MAX),
    payment_method NVARCHAR(20) CHECK (payment_method IN ('momo', 'bank_transfer', 'cash', 'other')) NOT NULL,
    transaction_id NVARCHAR(100),
    status NVARCHAR(20) CHECK (status IN ('pending', 'completed', 'failed', 'refunded')) DEFAULT 'pending',
    created_at DATETIME2 DEFAULT GETDATE(),
    completed_at DATETIME2 NULL,
    
    FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE SET NULL
);
GO

-- =====================================================
-- 10. BẢNG STATISTICS - Thống kê
-- =====================================================
CREATE TABLE statistics (
    stat_id INT IDENTITY(1,1) PRIMARY KEY,
    stat_date DATE NOT NULL,
    total_rescues INT DEFAULT 0,
    pending_rescues INT DEFAULT 0,
    completed_rescues INT DEFAULT 0,
    total_users INT DEFAULT 0,
    active_volunteers INT DEFAULT 0,
    total_donations DECIMAL(10, 2) DEFAULT 0,
    created_at DATETIME2 DEFAULT GETDATE(),
    
    CONSTRAINT unique_date UNIQUE (stat_date)
);
GO

-- =====================================================
-- 11. BẢNG RESCUE_HISTORY - Lịch sử cứu hộ
-- =====================================================
CREATE TABLE rescue_history (
    history_id INT IDENTITY(1,1) PRIMARY KEY,
    post_id INT NOT NULL,
    volunteer_id INT NOT NULL,
    action NVARCHAR(20) CHECK (action IN ('offered', 'accepted', 'started', 'completed', 'cancelled')) NOT NULL,
    note NVARCHAR(MAX),
    created_at DATETIME2 DEFAULT GETDATE(),
    
    FOREIGN KEY (post_id) REFERENCES rescue_posts(post_id) ON DELETE CASCADE,
    FOREIGN KEY (volunteer_id) REFERENCES users(user_id) ON DELETE NO ACTION
);
GO

-- =====================================================
-- 12. BẢNG POST_IMAGES - Hình ảnh bài đăng (nhiều ảnh)
-- =====================================================
CREATE TABLE post_images (
    image_id INT IDENTITY(1,1) PRIMARY KEY,
    post_id INT NOT NULL,
    image_url NVARCHAR(255) NOT NULL,
    is_primary BIT DEFAULT 0,
    display_order INT DEFAULT 0,
    created_at DATETIME2 DEFAULT GETDATE(),
    
    FOREIGN KEY (post_id) REFERENCES rescue_posts(post_id) ON DELETE CASCADE
);
GO

-- =====================================================
-- TẠO INDEXES
-- =====================================================

-- Index cho users
CREATE INDEX idx_email ON users(email);
CREATE INDEX idx_phone ON users(phone);
CREATE INDEX idx_user_role ON users(user_role);
CREATE INDEX idx_status ON users(status);

-- Index cho rescue_posts
CREATE INDEX idx_post_status ON rescue_posts(status);
CREATE INDEX idx_post_user_id ON rescue_posts(user_id);
CREATE INDEX idx_post_created_at ON rescue_posts(created_at);
CREATE INDEX idx_post_urgency ON rescue_posts(urgency_level);
CREATE INDEX idx_post_location ON rescue_posts(location);

-- Index cho rescue_volunteers
CREATE INDEX idx_volunteer_post_id ON rescue_volunteers(post_id);
CREATE INDEX idx_volunteer_user_id ON rescue_volunteers(user_id);
CREATE INDEX idx_volunteer_status ON rescue_volunteers(status);

-- Index cho comments
CREATE INDEX idx_comment_post_id ON comments(post_id);
CREATE INDEX idx_comment_user_id ON comments(user_id);
CREATE INDEX idx_comment_parent ON comments(parent_comment_id);

-- Index cho notifications
CREATE INDEX idx_notification_user_id ON notifications(user_id);
CREATE INDEX idx_notification_is_read ON notifications(is_read);
CREATE INDEX idx_notification_created_at ON notifications(created_at);

-- Index cho reports
CREATE INDEX idx_report_post_id ON reports(post_id);
CREATE INDEX idx_report_user_id ON reports(user_id);
CREATE INDEX idx_report_status ON reports(status);

-- Index cho team_members
CREATE INDEX idx_team_display_order ON team_members(display_order);
CREATE INDEX idx_team_is_active ON team_members(is_active);

-- Index cho donations
CREATE INDEX idx_donation_user_id ON donations(user_id);
CREATE INDEX idx_donation_status ON donations(status);
CREATE INDEX idx_donation_created_at ON donations(created_at);

-- Index cho statistics
CREATE INDEX idx_stat_date ON statistics(stat_date);

-- Index cho rescue_history
CREATE INDEX idx_history_post_id ON rescue_history(post_id);
CREATE INDEX idx_history_volunteer_id ON rescue_history(volunteer_id);
CREATE INDEX idx_history_created_at ON rescue_history(created_at);

-- Index cho post_images
CREATE INDEX idx_image_post_id ON post_images(post_id);
CREATE INDEX idx_image_is_primary ON post_images(is_primary);

-- =====================================================
-- VIEWS
-- =====================================================

-- View: Danh sách bài đăng với thông tin chi tiết
CREATE VIEW v_rescue_posts_detail AS
SELECT 
    rp.post_id,
    rp.title,
    rp.description,
    rp.location,
    rp.status,
    rp.urgency_level,
    rp.view_count,
    rp.created_at,
    u.user_id,
    u.full_name AS author_name,
    u.phone AS author_phone,
    at.type_name AS animal_type,
    at.type_emoji,
    COUNT(DISTINCT c.comment_id) AS comment_count,
    COUNT(DISTINCT rv.volunteer_id) AS volunteer_count
FROM rescue_posts rp
LEFT JOIN users u ON rp.user_id = u.user_id
LEFT JOIN animal_types at ON rp.animal_type_id = at.type_id
LEFT JOIN comments c ON rp.post_id = c.post_id
LEFT JOIN rescue_volunteers rv ON rp.post_id = rv.post_id
GROUP BY 
    rp.post_id, rp.title, rp.description, rp.location, rp.status, 
    rp.urgency_level, rp.view_count, rp.created_at, u.user_id, 
    u.full_name, u.phone, at.type_name, at.type_emoji;
GO

-- View: Thống kê người dùng
CREATE VIEW v_user_statistics AS
SELECT 
    u.user_id,
    u.full_name,
    u.email,
    u.user_role,
    COUNT(DISTINCT rp.post_id) AS total_posts,
    COUNT(DISTINCT rv.volunteer_id) AS total_volunteered,
    COUNT(DISTINCT c.comment_id) AS total_comments
FROM users u
LEFT JOIN rescue_posts rp ON u.user_id = rp.user_id
LEFT JOIN rescue_volunteers rv ON u.user_id = rv.user_id
LEFT JOIN comments c ON u.user_id = c.user_id
GROUP BY u.user_id, u.full_name, u.email, u.user_role;
GO

-- =====================================================
-- STORED PROCEDURES
-- =====================================================

-- Procedure: Tạo bài đăng mới
CREATE PROCEDURE sp_create_rescue_post
    @user_id INT,
    @title NVARCHAR(255),
    @description NVARCHAR(MAX),
    @animal_type_id INT,
    @location NVARCHAR(255),
    @contact_phone NVARCHAR(20),
    @urgency_level NVARCHAR(20),
    @latitude DECIMAL(10, 8),
    @longitude DECIMAL(11, 8)
AS
BEGIN
    INSERT INTO rescue_posts (
        user_id, title, description, animal_type_id, 
        location, contact_phone, urgency_level,
        latitude, longitude
    ) VALUES (
        @user_id, @title, @description, @animal_type_id,
        @location, @contact_phone, @urgency_level,
        @latitude, @longitude
    );
    
    SELECT SCOPE_IDENTITY() AS post_id;
END
GO

-- Procedure: Cập nhật trạng thái bài đăng
CREATE PROCEDURE sp_update_post_status
    @post_id INT,
    @status NVARCHAR(20),
    @volunteer_id INT
AS
BEGIN
    UPDATE rescue_posts 
    SET status = @status,
        rescued_at = CASE WHEN @status = 'rescued' THEN GETDATE() ELSE rescued_at END
    WHERE post_id = @post_id;
    
    -- Ghi lại lịch sử
    INSERT INTO rescue_history (post_id, volunteer_id, action, note)
    VALUES (@post_id, @volunteer_id, @status, 
            CONCAT('Cập nhật trạng thái thành ', @status));
END
GO

-- Procedure: Lấy thống kê tổng quan
CREATE PROCEDURE sp_get_dashboard_stats
AS
BEGIN
    SELECT 
        (SELECT COUNT(*) FROM rescue_posts WHERE status = 'waiting') AS sos_count,
        (SELECT COUNT(*) FROM rescue_posts WHERE status = 'rescued') AS rescued_count,
        (SELECT COUNT(*) FROM rescue_posts) AS total_posts,
        (SELECT COUNT(*) FROM users WHERE user_role = 'volunteer') AS volunteer_count,
        (SELECT COALESCE(SUM(amount), 0) FROM donations WHERE status = 'completed') AS total_donations;
END
GO

-- =====================================================
-- DỮ LIỆU MẪU
-- =====================================================

-- Thêm admin mặc định
INSERT INTO users (full_name, email, phone, password_hash, user_role, status, email_verified)
VALUES 
    (N'Admin', 'admin@pawhelp.com', '0901234567', 'admin123', 'admin', 'active', 1),
    (N'Nhân viên 1', 'staff01@pawhelp.com', '0909876543', 'staff123', 'volunteer', 'active', 1);
GO

-- Thêm loại động vật
INSERT INTO animal_types (type_name, type_emoji, description)
VALUES 
    (N'Chó', N'🐕', N'Chó bị bỏ rơi, lạc hoặc gặp nạn'),
    (N'Mèo', N'🐈', N'Mèo bị bỏ rơi, lạc hoặc gặp nạn'),
    (N'Chim', N'🐦', N'Chim bị thương hoặc gặp nạn'),
    (N'Thỏ', N'🐰', N'Thỏ cần cứu trợ'),
    (N'Khác', N'🐾', N'Các loại động vật khác');
GO

PRINT '✅ Database PawHelp đã được tạo thành công!';
SELECT COUNT(*) AS 'Số bảng' FROM sys.tables WHERE type = 'U';

