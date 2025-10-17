namespace DashboardPortal.Models
{
    public class Widget
    {
        public string WidgetId { get; set; }
        public string WidgetName { get; set; }
        public string WidgetUrl { get; set; }
        public string WidgetType { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<RoleWidget> RoleWidgets { get; set; }
        public ICollection<ViewWidget> ViewWidgets { get; set; }
    }
}
