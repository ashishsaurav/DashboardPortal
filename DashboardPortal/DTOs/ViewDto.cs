namespace DashboardPortal.DTOs
{
    public class ViewDto
    {
        public string ViewId { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public bool IsVisible { get; set; }
        public int OrderIndex { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<ReportDto> Reports { get; set; }
        public List<WidgetDto> Widgets { get; set; }
    }

    public class CreateViewDto
    {
        public string Name { get; set; }
        public bool IsVisible { get; set; } = true;
        public int OrderIndex { get; set; }
        public List<string> ReportIds { get; set; }
        public List<string> WidgetIds { get; set; }
    }

    public class UpdateViewDto
    {
        public string Name { get; set; }
        public bool IsVisible { get; set; }
        public int OrderIndex { get; set; }
    }
}
