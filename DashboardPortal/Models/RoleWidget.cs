namespace DashboardPortal.Models
{
    public class RoleWidget
    {
        public int Id { get; set; }
        public string RoleId { get; set; }
        public string WidgetId { get; set; }
        public int OrderIndex { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public UserRole Role { get; set; }
        public Widget Widget { get; set; }
    }
}
