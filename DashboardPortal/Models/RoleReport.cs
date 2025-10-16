namespace DashboardPortal.Models
{
    public class RoleReport
    {
        public int Id { get; set; }
        public string RoleId { get; set; }
        public string ReportId { get; set; }
        public int OrderIndex { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public UserRole Role { get; set; }
        public Report Report { get; set; }
    }
}
