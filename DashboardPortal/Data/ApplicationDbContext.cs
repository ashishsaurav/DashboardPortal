using Microsoft.EntityFrameworkCore;
using DashboardPortal.Models;

namespace DashboardPortal.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Widget> Widgets { get; set; }
        public DbSet<RoleReport> RoleReports { get; set; }
        public DbSet<RoleWidget> RoleWidgets { get; set; }
        public DbSet<ViewGroup> ViewGroups { get; set; }
        public DbSet<View> Views { get; set; }
        public DbSet<ViewGroupView> ViewGroupViews { get; set; }
        public DbSet<ViewReport> ViewReports { get; set; }
        public DbSet<ViewWidget> ViewWidgets { get; set; }
        public DbSet<LayoutCustomization> LayoutCustomizations { get; set; }
        public DbSet<NavigationSetting> NavigationSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // UserRole Configuration
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => e.RoleId);
                entity.Property(e => e.RoleId).HasMaxLength(50);
                entity.Property(e => e.RoleName).HasMaxLength(100).IsRequired();
            });

            // User Configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.UserId).HasMaxLength(50);
                entity.Property(e => e.Username).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
                entity.HasIndex(e => e.Email).IsUnique();

                entity.HasOne(e => e.Role)
                    .WithMany(r => r.Users)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Report Configuration
            modelBuilder.Entity<Report>(entity =>
            {
                entity.HasKey(e => e.ReportId);
                entity.Property(e => e.ReportId).HasMaxLength(50);
                entity.Property(e => e.ReportName).HasMaxLength(200).IsRequired();
                entity.Property(e => e.ReportUrl).HasMaxLength(500);
            });

            // Widget Configuration
            modelBuilder.Entity<Widget>(entity =>
            {
                entity.HasKey(e => e.WidgetId);
                entity.Property(e => e.WidgetId).HasMaxLength(50);
                entity.Property(e => e.WidgetName).HasMaxLength(200).IsRequired();
                entity.Property(e => e.WidgetUrl).HasMaxLength(500);
            });

            // RoleReport Configuration
            modelBuilder.Entity<RoleReport>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.RoleId, e.ReportId }).IsUnique();

                entity.HasOne(e => e.Role)
                    .WithMany(r => r.RoleReports)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Report)
                    .WithMany(r => r.RoleReports)
                    .HasForeignKey(e => e.ReportId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // RoleWidget Configuration
            modelBuilder.Entity<RoleWidget>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.RoleId, e.WidgetId }).IsUnique();

                entity.HasOne(e => e.Role)
                    .WithMany(r => r.RoleWidgets)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Widget)
                    .WithMany(w => w.RoleWidgets)
                    .HasForeignKey(e => e.WidgetId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ViewGroup Configuration
            modelBuilder.Entity<ViewGroup>(entity =>
            {
                entity.HasKey(e => e.ViewGroupId);
                entity.Property(e => e.ViewGroupId).HasMaxLength(50);
                entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.ViewGroups)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // View Configuration
            modelBuilder.Entity<View>(entity =>
            {
                entity.HasKey(e => e.ViewId);
                entity.Property(e => e.ViewId).HasMaxLength(50);
                entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Views)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ViewGroupView Configuration (Many-to-Many)
            modelBuilder.Entity<ViewGroupView>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.ViewGroupId, e.ViewId }).IsUnique();
                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.HasOne(e => e.ViewGroup)
                    .WithMany(vg => vg.ViewGroupViews)
                    .HasForeignKey(e => e.ViewGroupId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.View)
                    .WithMany(v => v.ViewGroupViews)
                    .HasForeignKey(e => e.ViewId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // ViewReport Configuration (Many-to-Many)
            modelBuilder.Entity<ViewReport>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.ViewId, e.ReportId }).IsUnique();

                entity.HasOne(e => e.View)
                    .WithMany(v => v.ViewReports)
                    .HasForeignKey(e => e.ViewId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Report)
                    .WithMany(r => r.ViewReports)
                    .HasForeignKey(e => e.ReportId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // ViewWidget Configuration (Many-to-Many)
            modelBuilder.Entity<ViewWidget>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.ViewId, e.WidgetId }).IsUnique();

                entity.HasOne(e => e.View)
                    .WithMany(v => v.ViewWidgets)
                    .HasForeignKey(e => e.ViewId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Widget)
                    .WithMany(w => w.ViewWidgets)
                    .HasForeignKey(e => e.WidgetId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // LayoutCustomization Configuration
            modelBuilder.Entity<LayoutCustomization>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.UserId, e.LayoutSignature }).IsUnique();
                entity.Property(e => e.LayoutSignature).HasMaxLength(255).IsRequired();

                entity.HasOne(e => e.User)
                    .WithMany(u => u.LayoutCustomizations)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // NavigationSetting Configuration
            modelBuilder.Entity<NavigationSetting>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.UserId).IsUnique();

                entity.HasOne(e => e.User)
                    .WithOne(u => u.NavigationSetting)
                    .HasForeignKey<NavigationSetting>(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}