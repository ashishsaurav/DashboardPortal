namespace DashboardPortal.DTOs
{
    public class ReportDto
    {
        public string ReportId { get; set; }
        public string ReportName { get; set; }
        public string ReportDescription { get; set; }
        public string ReportUrl { get; set; }
        public bool IsActive { get; set; }
        public int OrderIndex { get; set; }
    }

    public class CreateReportDto
    {
        public string ReportName { get; set; }
        public string ReportDescription { get; set; }
        public string ReportUrl { get; set; }
    }
}
