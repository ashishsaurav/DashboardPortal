namespace DashboardPortal.DTOs
{
    public class WidgetDto
    {
        public string WidgetId { get; set; }
        public string WidgetName { get; set; }
        public string WidgetDescription { get; set; }
        public string WidgetType { get; set; }
        public bool IsActive { get; set; }
        public int OrderIndex { get; set; }
    }

    public class CreateWidgetDto
    {
        public string WidgetName { get; set; }
        public string WidgetDescription { get; set; }
        public string WidgetType { get; set; }
    }
}
