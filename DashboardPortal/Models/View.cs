namespace DashboardPortal.Models
{
    public class View
    {
        public string ViewId { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public bool IsVisible { get; set; } = true;
        public int OrderIndex { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; }
        public ICollection<ViewGroupView> ViewGroupViews { get; set; }
        public ICollection<ViewReport> ViewReports { get; set; }
        public ICollection<ViewWidget> ViewWidgets { get; set; }
    }
}
