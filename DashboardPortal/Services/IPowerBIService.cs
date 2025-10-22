using DashboardPortal.DTOs;

namespace DashboardPortal.Services
{
    public interface IPowerBIService
    {
        Task<PowerBIDto> GetEmbedInfo(string workspaceId, string reportId);
        Task<string> GetAccessToken(string tenantId, string clientId, string clientSecret);
    }
}
