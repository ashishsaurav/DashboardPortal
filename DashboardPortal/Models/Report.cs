namespace DashboardPortal.Models
{
    public class Report
    {
        public string ReportId { get; set; }
        public string ReportName { get; set; }
        public string ReportUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<RoleReport> RoleReports { get; set; }
        public ICollection<ViewReport> ViewReports { get; set; }
    }
}
