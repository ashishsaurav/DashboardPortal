using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DashboardPortal.Data;
using DashboardPortal.Models;
using DashboardPortal.DTOs;
using System.Text.Json;

namespace DashboardPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ViewsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ViewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/views/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<ViewDto>>> GetUserViews(string userId)
        {
            var views = await _context.Views
                .Include(v => v.ViewReports).ThenInclude(vr => vr.Report)
                .Include(v => v.ViewWidgets).ThenInclude(vw => vw.Widget)
                .Where(v => v.UserId == userId)
                .OrderBy(v => v.OrderIndex)
                .ToListAsync();

            var viewDtos = views.Select(v => MapToDto(v)).ToList();
            return Ok(viewDtos);
        }

        // GET: api/views/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ViewDto>> GetView(string id, [FromQuery] string userId)
        {
            var view = await _context.Views
                .Include(v => v.ViewReports).ThenInclude(vr => vr.Report)
                .Include(v => v.ViewWidgets).ThenInclude(vw => vw.Widget)
                .FirstOrDefaultAsync(v => v.ViewId == id && v.UserId == userId);

            if (view == null)
                return NotFound(new { message = "View not found" });

            return Ok(MapToDto(view));
        }

        // POST: api/views
        [HttpPost]
        public async Task<ActionResult<ViewDto>> CreateView([FromBody] CreateViewRequest request)
        {
            var view = new View
            {
                ViewId = $"view-{request.UserId}-{Guid.NewGuid().ToString().Substring(0, 8)}",
                UserId = request.UserId,
                Name = request.Data.Name,
                IsVisible = request.Data.IsVisible,
                OrderIndex = request.Data.OrderIndex,
                CreatedBy = request.UserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Views.Add(view);

            // Add reports
            if (request.Data.ReportIds != null && request.Data.ReportIds.Any())
            {
                for (int i = 0; i < request.Data.ReportIds.Count; i++)
                {
                    _context.ViewReports.Add(new ViewReport
                    {
                        ViewId = view.ViewId,
                        ReportId = request.Data.ReportIds[i],
                        OrderIndex = i,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            // Add widgets
            if (request.Data.WidgetIds != null && request.Data.WidgetIds.Any())
            {
                for (int i = 0; i < request.Data.WidgetIds.Count; i++)
                {
                    _context.ViewWidgets.Add(new ViewWidget
                    {
                        ViewId = view.ViewId,
                        WidgetId = request.Data.WidgetIds[i],
                        OrderIndex = i,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            await _context.SaveChangesAsync();

            // Reload with includes
            view = await _context.Views
                .Include(v => v.ViewReports).ThenInclude(vr => vr.Report)
                .Include(v => v.ViewWidgets).ThenInclude(vw => vw.Widget)
                .FirstAsync(v => v.ViewId == view.ViewId);

            return CreatedAtAction(nameof(GetView), new { id = view.ViewId, userId = request.UserId }, MapToDto(view));
        }

        // PUT: api/views/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<ViewDto>> UpdateView(string id, [FromBody] UpdateViewRequest request)
        {
            var view = await _context.Views
                .FirstOrDefaultAsync(v => v.ViewId == id && v.UserId == request.UserId);

            if (view == null)
                return NotFound(new { message = "View not found" });

            view.Name = request.Data.Name;
            view.IsVisible = request.Data.IsVisible;
            view.OrderIndex = request.Data.OrderIndex;
            view.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            view = await _context.Views
                .Include(v => v.ViewReports).ThenInclude(vr => vr.Report)
                .Include(v => v.ViewWidgets).ThenInclude(vw => vw.Widget)
                .FirstAsync(v => v.ViewId == id);

            return Ok(MapToDto(view));
        }

        // DELETE: api/views/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteView(string id, [FromQuery] string userId)
        {
            var view = await _context.Views
                .FirstOrDefaultAsync(v => v.ViewId == id && v.UserId == userId);

            if (view == null)
                return NotFound(new { message = "View not found" });

            _context.Views.Remove(view);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/views/{id}/reports
        [HttpPost("{id}/reports")]
        public async Task<IActionResult> AddReportsToView(string id, [FromBody] AddReportsToViewRequest request)
        {
            var view = await _context.Views
                .FirstOrDefaultAsync(v => v.ViewId == id && v.UserId == request.UserId);

            if (view == null)
                return NotFound(new { message = "View not found" });

            var maxOrder = await _context.ViewReports
                .Where(vr => vr.ViewId == id)
                .MaxAsync(vr => (int?)vr.OrderIndex) ?? -1;

            foreach (var reportId in request.ReportIds)
            {
                var exists = await _context.ViewReports
                    .AnyAsync(vr => vr.ViewId == id && vr.ReportId == reportId);

                if (!exists)
                {
                    _context.ViewReports.Add(new ViewReport
                    {
                        ViewId = id,
                        ReportId = reportId,
                        OrderIndex = ++maxOrder,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Reports added successfully" });
        }

        // DELETE: api/views/{viewId}/reports/{reportId}
        [HttpDelete("{viewId}/reports/{reportId}")]
        public async Task<IActionResult> RemoveReportFromView(string viewId, string reportId, [FromQuery] string userId)
        {
            var view = await _context.Views
                .FirstOrDefaultAsync(v => v.ViewId == viewId && v.UserId == userId);

            if (view == null)
                return NotFound(new { message = "View not found" });

            var viewReport = await _context.ViewReports
                .FirstOrDefaultAsync(vr => vr.ViewId == viewId && vr.ReportId == reportId);

            if (viewReport == null)
                return NotFound(new { message = "Report not found in view" });

            _context.ViewReports.Remove(viewReport);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/views/{id}/widgets
        [HttpPost("{id}/widgets")]
        public async Task<IActionResult> AddWidgetsToView(string id, [FromBody] AddWidgetsToViewRequest request)
        {
            var view = await _context.Views
                .FirstOrDefaultAsync(v => v.ViewId == id && v.UserId == request.UserId);

            if (view == null)
                return NotFound(new { message = "View not found" });

            var maxOrder = await _context.ViewWidgets
                .Where(vw => vw.ViewId == id)
                .MaxAsync(vw => (int?)vw.OrderIndex) ?? -1;

            foreach (var widgetId in request.WidgetIds)
            {
                var exists = await _context.ViewWidgets
                    .AnyAsync(vw => vw.ViewId == id && vw.WidgetId == widgetId);

                if (!exists)
                {
                    _context.ViewWidgets.Add(new ViewWidget
                    {
                        ViewId = id,
                        WidgetId = widgetId,
                        OrderIndex = ++maxOrder,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Widgets added successfully" });
        }

        // DELETE: api/views/{viewId}/widgets/{widgetId}
        [HttpDelete("{viewId}/widgets/{widgetId}")]
        public async Task<IActionResult> RemoveWidgetFromView(string viewId, string widgetId, [FromQuery] string userId)
        {
            var view = await _context.Views
                .FirstOrDefaultAsync(v => v.ViewId == viewId && v.UserId == userId);

            if (view == null)
                return NotFound(new { message = "View not found" });

            var viewWidget = await _context.ViewWidgets
                .FirstOrDefaultAsync(vw => vw.ViewId == viewId && vw.WidgetId == widgetId);

            if (viewWidget == null)
                return NotFound(new { message = "Widget not found in view" });

            _context.ViewWidgets.Remove(viewWidget);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/views/{id}/reports/reorder
        [HttpPost("{id}/reports/reorder")]
        public async Task<IActionResult> ReorderReports(string id, [FromBody] ReorderItemsRequest request)
        {
            var view = await _context.Views
                .FirstOrDefaultAsync(v => v.ViewId == id && v.UserId == request.UserId);

            if (view == null)
                return NotFound(new { message = "View not found" });

            var viewReports = await _context.ViewReports
                .Where(vr => vr.ViewId == id)
                .ToListAsync();

            foreach (var item in request.Items)
            {
                var vr = viewReports.FirstOrDefault(vr => vr.ReportId == item.Id);
                if (vr != null)
                {
                    vr.OrderIndex = item.OrderIndex;
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Reports reordered successfully" });
        }

        // POST: api/views/{id}/widgets/reorder
        [HttpPost("{id}/widgets/reorder")]
        public async Task<IActionResult> ReorderWidgets(string id, [FromBody] ReorderItemsRequest request)
        {
            var view = await _context.Views
                .FirstOrDefaultAsync(v => v.ViewId == id && v.UserId == request.UserId);

            if (view == null)
                return NotFound(new { message = "View not found" });

            var viewWidgets = await _context.ViewWidgets
                .Where(vw => vw.ViewId == id)
                .ToListAsync();

            foreach (var item in request.Items)
            {
                var vw = viewWidgets.FirstOrDefault(vw => vw.WidgetId == item.Id);
                if (vw != null)
                {
                    vw.OrderIndex = item.OrderIndex;
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Widgets reordered successfully" });
        }

        private ViewDto MapToDto(View view)
        {
            return new ViewDto
            {
                ViewId = view.ViewId,
                UserId = view.UserId,
                Name = view.Name,
                IsVisible = view.IsVisible,
                OrderIndex = view.OrderIndex,
                CreatedBy = view.CreatedBy,
                CreatedAt = view.CreatedAt,
                UpdatedAt = view.UpdatedAt,
                Reports = view.ViewReports?
                    .OrderBy(vr => vr.OrderIndex)
                    .Select(vr => new ReportDto
                    {
                        ReportId = vr.Report.ReportId,
                        ReportName = vr.Report.ReportName,
                        ReportUrl = vr.Report.ReportUrl,
                        IsActive = vr.Report.IsActive,
                        OrderIndex = vr.OrderIndex
                    }).ToList() ?? new List<ReportDto>(),
                Widgets = view.ViewWidgets?
                    .OrderBy(vw => vw.OrderIndex)
                    .Select(vw => new WidgetDto
                    {
                        WidgetId = vw.Widget.WidgetId,
                        WidgetName = vw.Widget.WidgetName,
                        WidgetUrl = vw.Widget.WidgetUrl,
                        IsActive = vw.Widget.IsActive,
                        OrderIndex = vw.OrderIndex
                    }).ToList() ?? new List<WidgetDto>()
            };
        }
    }

    // Request DTOs
    public class CreateViewRequest
    {
        public string UserId { get; set; }
        public CreateViewDto Data { get; set; }
    }

    public class UpdateViewRequest
    {
        public string UserId { get; set; }
        public UpdateViewDto Data { get; set; }
    }

    public class AddReportsToViewRequest
    {
        public string UserId { get; set; }
        public List<string> ReportIds { get; set; }
    }

    public class AddWidgetsToViewRequest
    {
        public string UserId { get; set; }
        public List<string> WidgetIds { get; set; }
    }

    public class ReorderItemsRequest
    {
        public string UserId { get; set; }
        public List<ReorderItemDto> Items { get; set; }
    }
}