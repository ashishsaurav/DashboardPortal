namespace DashboardPortal.Models
{
    public class UserRole
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<User> Users { get; set; }
        public ICollection<RoleReport> RoleReports { get; set; }
        public ICollection<RoleWidget> RoleWidgets { get; set; }
    }
}
