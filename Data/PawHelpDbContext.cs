using Microsoft.EntityFrameworkCore;
using PawHelp.Models.Entities;

namespace PawHelp.Data;

public class PawHelpDbContext : DbContext
{
    public PawHelpDbContext(DbContextOptions<PawHelpDbContext> options) : base(options)
    {
    }

    // DbSets
    public DbSet<User> Users { get; set; }
    public DbSet<AnimalType> AnimalTypes { get; set; }
    public DbSet<RescuePost> RescuePosts { get; set; }
    public DbSet<RescueVolunteer> RescueVolunteers { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Report> Reports { get; set; }
    public DbSet<TeamMember> TeamMembers { get; set; }
    public DbSet<Donation> Donations { get; set; }
    public DbSet<Statistic> Statistics { get; set; }
    public DbSet<RescueHistory> RescueHistories { get; set; }
    public DbSet<PostImage> PostImages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure table names (lowercase với underscore)
        modelBuilder.Entity<User>().ToTable("users");
        modelBuilder.Entity<AnimalType>().ToTable("animal_types");
        modelBuilder.Entity<RescuePost>().ToTable("rescue_posts");
        modelBuilder.Entity<RescueVolunteer>().ToTable("rescue_volunteers");
        modelBuilder.Entity<Comment>().ToTable("comments");
        modelBuilder.Entity<Notification>().ToTable("notifications");
        modelBuilder.Entity<Report>().ToTable("reports");
        modelBuilder.Entity<TeamMember>().ToTable("team_members");
        modelBuilder.Entity<Donation>().ToTable("donations");
        modelBuilder.Entity<Statistic>().ToTable("statistics");
        modelBuilder.Entity<RescueHistory>().ToTable("rescue_history");
        modelBuilder.Entity<PostImage>().ToTable("post_images");

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.FullName).HasColumnName("full_name").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Phone).HasColumnName("phone").HasMaxLength(20);
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
            entity.Property(e => e.AvatarUrl).HasColumnName("avatar_url").HasMaxLength(255);
            entity.Property(e => e.UserRole).HasColumnName("user_role").HasMaxLength(20).HasDefaultValue("user");
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(20).HasDefaultValue("active");
            entity.Property(e => e.EmailVerified).HasColumnName("email_verified").HasDefaultValue(false);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.LastLogin).HasColumnName("last_login");

            entity.HasIndex(e => e.Email).HasDatabaseName("idx_email");
            entity.HasIndex(e => e.Phone).HasDatabaseName("idx_phone");
            entity.HasIndex(e => e.UserRole).HasDatabaseName("idx_user_role");
            entity.HasIndex(e => e.Status).HasDatabaseName("idx_status");
        });

        // AnimalType configuration
        modelBuilder.Entity<AnimalType>(entity =>
        {
            entity.HasKey(e => e.TypeId);
            entity.Property(e => e.TypeId).HasColumnName("type_id");
            entity.Property(e => e.TypeName).HasColumnName("type_name").HasMaxLength(50).IsRequired();
            entity.Property(e => e.TypeEmoji).HasColumnName("type_emoji").HasMaxLength(10);
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("GETDATE()");
        });

        // RescuePost configuration
        modelBuilder.Entity<RescuePost>(entity =>
        {
            entity.HasKey(e => e.PostId);
            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Title).HasColumnName("title").HasMaxLength(255).IsRequired();
            entity.Property(e => e.Description).HasColumnName("description").IsRequired();
            entity.Property(e => e.AnimalTypeId).HasColumnName("animal_type_id");
            entity.Property(e => e.Location).HasColumnName("location").HasMaxLength(255).IsRequired();
            entity.Property(e => e.Latitude).HasColumnName("latitude").HasPrecision(10, 8);
            entity.Property(e => e.Longitude).HasColumnName("longitude").HasPrecision(11, 8);
            entity.Property(e => e.ImageUrl).HasColumnName("image_url").HasMaxLength(255);
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(20).HasDefaultValue("waiting");
            entity.Property(e => e.UrgencyLevel).HasColumnName("urgency_level").HasMaxLength(20).HasDefaultValue("medium");
            entity.Property(e => e.ContactPhone).HasColumnName("contact_phone").HasMaxLength(20);
            entity.Property(e => e.ViewCount).HasColumnName("view_count").HasDefaultValue(0);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.RescuedAt).HasColumnName("rescued_at");

            entity.HasOne(e => e.User).WithMany(u => u.RescuePosts).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.AnimalType).WithMany(a => a.RescuePosts).HasForeignKey(e => e.AnimalTypeId).OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.Status).HasDatabaseName("idx_post_status");
            entity.HasIndex(e => e.UserId).HasDatabaseName("idx_post_user_id");
            entity.HasIndex(e => e.CreatedAt).HasDatabaseName("idx_post_created_at");
            entity.HasIndex(e => e.UrgencyLevel).HasDatabaseName("idx_post_urgency");
            entity.HasIndex(e => e.Location).HasDatabaseName("idx_post_location");
        });

        // RescueVolunteer configuration
        modelBuilder.Entity<RescueVolunteer>(entity =>
        {
            entity.HasKey(e => e.VolunteerId);
            entity.Property(e => e.VolunteerId).HasColumnName("volunteer_id");
            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(20).HasDefaultValue("offered");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("GETDATE()");

            entity.HasOne(e => e.Post).WithMany(p => p.RescueVolunteers).HasForeignKey(e => e.PostId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.User).WithMany(u => u.RescueVolunteers).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.NoAction);

            entity.HasIndex(e => new { e.PostId, e.UserId }).IsUnique().HasDatabaseName("unique_volunteer");
            entity.HasIndex(e => e.PostId).HasDatabaseName("idx_volunteer_post_id");
            entity.HasIndex(e => e.UserId).HasDatabaseName("idx_volunteer_user_id");
            entity.HasIndex(e => e.Status).HasDatabaseName("idx_volunteer_status");
        });

        // Comment configuration
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId);
            entity.Property(e => e.CommentId).HasColumnName("comment_id");
            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Content).HasColumnName("content").IsRequired();
            entity.Property(e => e.ParentCommentId).HasColumnName("parent_comment_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("GETDATE()");

            entity.HasOne(e => e.Post).WithMany(p => p.Comments).HasForeignKey(e => e.PostId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.User).WithMany(u => u.Comments).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(e => e.ParentComment).WithMany(c => c.Replies).HasForeignKey(e => e.ParentCommentId).OnDelete(DeleteBehavior.NoAction);

            entity.HasIndex(e => e.PostId).HasDatabaseName("idx_comment_post_id");
            entity.HasIndex(e => e.UserId).HasDatabaseName("idx_comment_user_id");
            entity.HasIndex(e => e.ParentCommentId).HasDatabaseName("idx_comment_parent");
        });

        // Other entities continue with similar pattern...
        ConfigureAdditionalEntities(modelBuilder);
    }

    private void ConfigureAdditionalEntities(ModelBuilder modelBuilder)
    {
        // Notification
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId);
            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Title).HasColumnName("title").HasMaxLength(255).IsRequired();
            entity.Property(e => e.Message).HasColumnName("message").IsRequired();
            entity.Property(e => e.Type).HasColumnName("type").HasMaxLength(20).HasDefaultValue("system");
            entity.Property(e => e.RelatedPostId).HasColumnName("related_post_id");
            entity.Property(e => e.Icon).HasColumnName("icon").HasMaxLength(50);
            entity.Property(e => e.IsRead).HasColumnName("is_read").HasDefaultValue(false);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("GETDATE()");

            entity.HasOne(e => e.User).WithMany(u => u.Notifications).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.RelatedPost).WithMany().HasForeignKey(e => e.RelatedPostId).OnDelete(DeleteBehavior.NoAction);

            entity.HasIndex(e => e.UserId).HasDatabaseName("idx_notification_user_id");
            entity.HasIndex(e => e.IsRead).HasDatabaseName("idx_notification_is_read");
            entity.HasIndex(e => e.CreatedAt).HasDatabaseName("idx_notification_created_at");
        });

        // Report
        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.ReportId);
            entity.Property(e => e.ReportId).HasColumnName("report_id");
            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Reason).HasColumnName("reason").HasMaxLength(20).IsRequired();
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(20).HasDefaultValue("pending");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.ResolvedAt).HasColumnName("resolved_at");

            entity.HasOne(e => e.Post).WithMany().HasForeignKey(e => e.PostId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.NoAction);

            entity.HasIndex(e => e.PostId).HasDatabaseName("idx_report_post_id");
            entity.HasIndex(e => e.UserId).HasDatabaseName("idx_report_user_id");
            entity.HasIndex(e => e.Status).HasDatabaseName("idx_report_status");
        });

        // TeamMember
        modelBuilder.Entity<TeamMember>(entity =>
        {
            entity.HasKey(e => e.MemberId);
            entity.Property(e => e.MemberId).HasColumnName("member_id");
            entity.Property(e => e.FullName).HasColumnName("full_name").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Role).HasColumnName("role").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Position).HasColumnName("position").HasMaxLength(100);
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.AvatarUrl).HasColumnName("avatar_url").HasMaxLength(255);
            entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(100);
            entity.Property(e => e.Phone).HasColumnName("phone").HasMaxLength(20);
            entity.Property(e => e.TeamName).HasColumnName("team_name").HasMaxLength(100);
            entity.Property(e => e.DisplayOrder).HasColumnName("display_order").HasDefaultValue(0);
            entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("GETDATE()");

            entity.HasIndex(e => e.DisplayOrder).HasDatabaseName("idx_team_display_order");
            entity.HasIndex(e => e.IsActive).HasDatabaseName("idx_team_is_active");
        });

        // Donation
        modelBuilder.Entity<Donation>(entity =>
        {
            entity.HasKey(e => e.DonationId);
            entity.Property(e => e.DonationId).HasColumnName("donation_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.DonorName).HasColumnName("donor_name").HasMaxLength(100);
            entity.Property(e => e.DonorEmail).HasColumnName("donor_email").HasMaxLength(100);
            entity.Property(e => e.DonorPhone).HasColumnName("donor_phone").HasMaxLength(20);
            entity.Property(e => e.Amount).HasColumnName("amount").HasPrecision(10, 2).IsRequired();
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.PaymentMethod).HasColumnName("payment_method").HasMaxLength(20).IsRequired();
            entity.Property(e => e.TransactionId).HasColumnName("transaction_id").HasMaxLength(100);
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(20).HasDefaultValue("pending");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.CompletedAt).HasColumnName("completed_at");

            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.UserId).HasDatabaseName("idx_donation_user_id");
            entity.HasIndex(e => e.Status).HasDatabaseName("idx_donation_status");
            entity.HasIndex(e => e.CreatedAt).HasDatabaseName("idx_donation_created_at");
        });

        // Statistic
        modelBuilder.Entity<Statistic>(entity =>
        {
            entity.HasKey(e => e.StatId);
            entity.Property(e => e.StatId).HasColumnName("stat_id");
            entity.Property(e => e.StatDate).HasColumnName("stat_date").IsRequired();
            entity.Property(e => e.TotalRescues).HasColumnName("total_rescues").HasDefaultValue(0);
            entity.Property(e => e.PendingRescues).HasColumnName("pending_rescues").HasDefaultValue(0);
            entity.Property(e => e.CompletedRescues).HasColumnName("completed_rescues").HasDefaultValue(0);
            entity.Property(e => e.TotalUsers).HasColumnName("total_users").HasDefaultValue(0);
            entity.Property(e => e.ActiveVolunteers).HasColumnName("active_volunteers").HasDefaultValue(0);
            entity.Property(e => e.TotalDonations).HasColumnName("total_donations").HasPrecision(10, 2).HasDefaultValue(0);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("GETDATE()");

            entity.HasIndex(e => e.StatDate).IsUnique().HasDatabaseName("unique_date");
            entity.HasIndex(e => e.StatDate).HasDatabaseName("idx_stat_date");
        });

        // RescueHistory
        modelBuilder.Entity<RescueHistory>(entity =>
        {
            entity.HasKey(e => e.HistoryId);
            entity.Property(e => e.HistoryId).HasColumnName("history_id");
            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.VolunteerId).HasColumnName("volunteer_id");
            entity.Property(e => e.Action).HasColumnName("action").HasMaxLength(20).IsRequired();
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("GETDATE()");

            entity.HasOne(e => e.Post).WithMany().HasForeignKey(e => e.PostId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Volunteer).WithMany().HasForeignKey(e => e.VolunteerId).OnDelete(DeleteBehavior.NoAction);

            entity.HasIndex(e => e.PostId).HasDatabaseName("idx_history_post_id");
            entity.HasIndex(e => e.VolunteerId).HasDatabaseName("idx_history_volunteer_id");
            entity.HasIndex(e => e.CreatedAt).HasDatabaseName("idx_history_created_at");
        });

        // PostImage
        modelBuilder.Entity<PostImage>(entity =>
        {
            entity.HasKey(e => e.ImageId);
            entity.Property(e => e.ImageId).HasColumnName("image_id");
            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.ImageUrl).HasColumnName("image_url").HasMaxLength(255).IsRequired();
            entity.Property(e => e.IsPrimary).HasColumnName("is_primary").HasDefaultValue(false);
            entity.Property(e => e.DisplayOrder).HasColumnName("display_order").HasDefaultValue(0);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("GETDATE()");

            entity.HasOne(e => e.Post).WithMany(p => p.PostImages).HasForeignKey(e => e.PostId).OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.PostId).HasDatabaseName("idx_image_post_id");
            entity.HasIndex(e => e.IsPrimary).HasDatabaseName("idx_image_is_primary");
        });
    }
}

