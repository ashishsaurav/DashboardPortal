namespace DashboardPortal.DTOs
{
    public class ReorderDto
    {
        public List<ReorderItemDto> Items { get; set; }
    }

    public class ReorderItemDto
    {
        public string Id { get; set; }
        public int OrderIndex { get; set; }
    }
}
