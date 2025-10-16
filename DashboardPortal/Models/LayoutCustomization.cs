namespace DashboardPortal.Models
{
    public class LayoutCustomization
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string LayoutSignature { get; set; }
        public string LayoutData { get; set; }
        public long? Timestamp { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; }
    }
}
