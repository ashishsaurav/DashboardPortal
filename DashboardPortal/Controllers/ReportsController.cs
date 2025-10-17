using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DashboardPortal.Data;
using DashboardPortal.Models;
using DashboardPortal.DTOs;

namespace DashboardPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/reports
        [HttpGet]
        public async Task<ActionResult<List<ReportDto>>> GetAllReports()
        {
            var reports = await _context.Reports
                .Where(r => r.IsActive)
                .OrderBy(r => r.ReportName)
                .ToListAsync();

            return Ok(reports.Select(r => MapToDto(r)).ToList());
        }

        // GET: api/reports/role/{roleId}
        [HttpGet("role/{roleId}")]
        public async Task<ActionResult<List<ReportDto>>> GetReportsByRole(string roleId)
        {
            var reports = await _context.RoleReports
                .Include(rr => rr.Report)
                .Where(rr => rr.RoleId == roleId && rr.Report.IsActive)
                .OrderBy(rr => rr.OrderIndex)
                .Select(rr => new ReportDto
                {
                    ReportId = rr.Report.ReportId,
                    ReportName = rr.Report.ReportName,
                    ReportUrl = rr.Report.ReportUrl,
                    IsActive = rr.Report.IsActive,
                    OrderIndex = rr.OrderIndex
                })
                .ToListAsync();

            return Ok(reports);
        }

        // GET: api/reports/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ReportDto>> GetReport(string id)
        {
            var report = await _context.Reports.FindAsync(id);

            if (report == null)
                return NotFound(new { message = "Report not found" });

            return Ok(MapToDto(report));
        }

        // POST: api/reports
        [HttpPost]
        public async Task<ActionResult<ReportDto>> CreateReport([FromBody] CreateReportDto dto)
        {
            var report = new Report
            {
                ReportId = $"report-{Guid.NewGuid().ToString().Substring(0, 8)}",
                ReportName = dto.ReportName,
                ReportUrl = dto.ReportUrl,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Reports.Add(report);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReport), new { id = report.ReportId }, MapToDto(report));
        }

        // PUT: api/reports/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<ReportDto>> UpdateReport(string id, [FromBody] CreateReportDto dto)
        {
            var report = await _context.Reports.FindAsync(id);

            if (report == null)
                return NotFound(new { message = "Report not found" });

            report.ReportName = dto.ReportName;
            report.ReportUrl = dto.ReportUrl;
            report.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(MapToDto(report));
        }

        // DELETE: api/reports/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReport(string id)
        {
            var report = await _context.Reports.FindAsync(id);

            if (report == null)
                return NotFound(new { message = "Report not found" });

            _context.Reports.Remove(report);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/reports/role/{roleId}/assign
        [HttpPost("role/{roleId}/assign")]
        public async Task<IActionResult> AssignReportsToRole(string roleId, [FromBody] AssignReportsRequest request)
        {
            var role = await _context.UserRoles.FindAsync(roleId);
            if (role == null)
                return NotFound(new { message = "Role not found" });

            var maxOrder = await _context.RoleReports
                .Where(rr => rr.RoleId == roleId)
                .MaxAsync(rr => (int?)rr.OrderIndex) ?? -1;

            foreach (var reportId in request.ReportIds)
            {
                var exists = await _context.RoleReports
                    .AnyAsync(rr => rr.RoleId == roleId && rr.ReportId == reportId);

                if (!exists)
                {
                    _context.RoleReports.Add(new RoleReport
                    {
                        RoleId = roleId,
                        ReportId = reportId,
                        OrderIndex = ++maxOrder,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Reports assigned successfully" });
        }

        // DELETE: api/reports/role/{roleId}/unassign/{reportId}
        [HttpDelete("role/{roleId}/unassign/{reportId}")]
        public async Task<IActionResult> UnassignReportFromRole(string roleId, string reportId)
        {
            var roleReport = await _context.RoleReports
                .FirstOrDefaultAsync(rr => rr.RoleId == roleId && rr.ReportId == reportId);

            if (roleReport == null)
                return NotFound(new { message = "Assignment not found" });

            _context.RoleReports.Remove(roleReport);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private ReportDto MapToDto(Report report)
        {
            return new ReportDto
            {
                ReportId = report.ReportId,
                ReportName = report.ReportName,
                ReportUrl = report.ReportUrl,
                IsActive = report.IsActive
            };
        }
    }

    public class AssignReportsRequest
    {
        public List<string> ReportIds { get; set; }
    }
}