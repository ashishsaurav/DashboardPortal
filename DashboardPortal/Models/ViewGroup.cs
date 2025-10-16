namespace DashboardPortal.Models
{
    public class ViewGroup
    {
        public string ViewGroupId { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public bool IsVisible { get; set; } = true;
        public bool IsDefault { get; set; } = false;
        public int OrderIndex { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; }
        public ICollection<ViewGroupView> ViewGroupViews { get; set; }
    }
}
