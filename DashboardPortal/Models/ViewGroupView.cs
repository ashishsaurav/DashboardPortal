namespace DashboardPortal.Models
{
    public class ViewGroupView
    {
        public int Id { get; set; }
        public string ViewGroupId { get; set; }
        public string ViewId { get; set; }
        public int OrderIndex { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ViewGroup ViewGroup { get; set; }
        public View View { get; set; }
    }
}
