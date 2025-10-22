namespace DashboardPortal.Models
{
    public class NavigationSetting
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string ViewGroupOrder { get; set; }
        public string ViewOrders { get; set; }
        public string HiddenViewGroups { get; set; }
        public string HiddenViews { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public User User { get; set; }
        public string ExpandedViewGroups { get; set; }  // ✅ NEW - JSON array
        public bool? IsNavigationCollapsed { get; set; } // ✅ NEW
    }
}