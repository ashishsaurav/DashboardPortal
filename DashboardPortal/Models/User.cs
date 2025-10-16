namespace DashboardPortal.Models
{
    public class User
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string RoleId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public UserRole Role { get; set; }
        public ICollection<ViewGroup> ViewGroups { get; set; }
        public ICollection<View> Views { get; set; }
        public ICollection<LayoutCustomization> LayoutCustomizations { get; set; }
        public NavigationSetting NavigationSetting { get; set; }
    }
}
