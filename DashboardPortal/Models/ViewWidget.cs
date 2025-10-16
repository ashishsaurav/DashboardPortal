namespace DashboardPortal.Models
{
    public class ViewWidget
    {
        public int Id { get; set; }
        public string ViewId { get; set; }
        public string WidgetId { get; set; }
        public int OrderIndex { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public View View { get; set; }
        public Widget Widget { get; set; }
    }
}
