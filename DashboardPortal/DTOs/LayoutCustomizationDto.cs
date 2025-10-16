namespace DashboardPortal.DTOs
{
    public class LayoutCustomizationDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string LayoutSignature { get; set; }
        public string LayoutData { get; set; }
        public long? Timestamp { get; set; }
    }

    public class SaveLayoutDto
    {
        public string LayoutSignature { get; set; }
        public string LayoutData { get; set; }
        public long? Timestamp { get; set; }
    }
}
