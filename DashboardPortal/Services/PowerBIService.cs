using DashboardPortal.DTOs;
using Microsoft.Identity.Client;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using Microsoft.Rest;

namespace DashboardPortal.Services
{
    public class PowerBIService : IPowerBIService
    {
        private readonly IConfiguration _configuration;

        public PowerBIService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<PowerBIDto> GetEmbedInfo(string workspaceId, string reportId)
        {
            try
            {
                // Get configuration values
                var tenantId = _configuration["PowerBI:TenantId"];
                var clientId = _configuration["PowerBI:ClientId"];
                var clientSecret = _configuration["PowerBI:ClientSecret"];
                var apiUrl = "https://api.powerbi.com/";

                // Get access token
                string accessToken = await GetAccessToken(tenantId, clientId, clientSecret);

                // Create Power BI Client
                var tokenCredentials = new TokenCredentials(accessToken, "Bearer");
                var client = new PowerBIClient(new Uri(apiUrl), tokenCredentials);

                // Get the report to retrieve its embed URL and dataset ID
                var report = await client.Reports.GetReportInGroupAsync(Guid.Parse(workspaceId), Guid.Parse(reportId));

                // Generate embed token with user context
                var generateTokenRequestParameters = new GenerateTokenRequest(
                    accessLevel: "View"
                );

                var tokenResponse = await client.Reports.GenerateTokenInGroupAsync(
                    Guid.Parse(workspaceId),
                    Guid.Parse(reportId),
                    generateTokenRequestParameters);

                var expireTime = tokenResponse.Expiration.Subtract(DateTime.UtcNow).TotalMilliseconds.ToString();

                return new PowerBIDto
                {
                    ReportId = reportId,
                    EmbedUrl = report.EmbedUrl,
                    EmbedToken = tokenResponse.Token,
                    TokenExpiration = expireTime
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<string> GetAccessToken(string tenantId, string clientId, string clientSecret)
        {
            // Configure the MSAL client
            IConfidentialClientApplication app = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithClientSecret(clientSecret)
                .WithAuthority(new Uri($"https://login.microsoftonline.com/{tenantId}"))
                .Build();

            // Define the resource scope
            string[] scopes = new string[] { "https://analysis.windows.net/powerbi/api/.default" };

            try
            {
                // Acquire token using client credentials flow
                var result = await app.AcquireTokenForClient(scopes).ExecuteAsync();
                return result.AccessToken;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
    }
}
