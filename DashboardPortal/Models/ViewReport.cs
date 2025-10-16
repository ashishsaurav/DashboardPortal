namespace DashboardPortal.Models
{
    public class ViewReport
    {
        public int Id { get; set; }
        public string ViewId { get; set; }
        public string ReportId { get; set; }
        public int OrderIndex { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public View View { get; set; }
        public Report Report { get; set; }
    }
}
