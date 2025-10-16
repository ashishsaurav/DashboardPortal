namespace DashboardPortal.DTOs
{
    public class NavigationSettingDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public List<string> ViewGroupOrder { get; set; }
        public Dictionary<string, List<string>> ViewOrders { get; set; }
        public List<string> HiddenViewGroups { get; set; }
        public List<string> HiddenViews { get; set; }
    }

    public class UpdateNavigationSettingDto
    {
        public List<string> ViewGroupOrder { get; set; }
        public Dictionary<string, List<string>> ViewOrders { get; set; }
        public List<string> HiddenViewGroups { get; set; }
        public List<string> HiddenViews { get; set; }
    }
}
