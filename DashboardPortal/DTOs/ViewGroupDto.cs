namespace DashboardPortal.DTOs
{
    public class ViewGroupDto
    {
        public string ViewGroupId { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public bool IsVisible { get; set; }
        public bool IsDefault { get; set; }
        public int OrderIndex { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<ViewDto> Views { get; set; }
    }

    public class CreateViewGroupDto
    {
        public string Name { get; set; }
        public bool IsVisible { get; set; } = true;
        public bool IsDefault { get; set; } = false;
        public int OrderIndex { get; set; }
    }

    public class UpdateViewGroupDto
    {
        public string Name { get; set; }
        public bool IsVisible { get; set; }
        public bool IsDefault { get; set; }
        public int OrderIndex { get; set; }
    }
}
