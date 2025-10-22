using DashboardPortal.Services;
using Microsoft.AspNetCore.Mvc;

namespace DashboardPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PowerBIController : ControllerBase
    {
        private readonly IPowerBIService _powerBIService;

        public PowerBIController(IPowerBIService powerBIService)
        {
            _powerBIService = powerBIService;
        }

        // GET: api/powerbi/{workspaceId}
        [HttpGet("{workspaceId}")]
        public async Task<IActionResult> GetPowerBIToken(string workspaceId, string reportId)
        {
            try
            {
                // Validate required parameters
                if (string.IsNullOrEmpty(workspaceId))
                {
                    return BadRequest("WorkspaceId is required");
                }

                if (string.IsNullOrEmpty(reportId))
                {
                    return BadRequest("ReportId is required");
                }

                // Get embed info
                var embedInfo = await _powerBIService.GetEmbedInfo(workspaceId, reportId);
                return Ok(embedInfo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
