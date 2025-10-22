namespace DashboardPortal.DTOs
{
    public class PowerBIDto
    {
        public string ReportId { get; set; } = string.Empty;
        public string EmbedUrl { get; set; } = string.Empty;
        public string EmbedToken { get; set; } = string.Empty;
        public string TokenExpiration { get; set; } = string.Empty;
    }
}
