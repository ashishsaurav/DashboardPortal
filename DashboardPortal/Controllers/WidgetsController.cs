using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DashboardPortal.Data;
using DashboardPortal.Models;
using DashboardPortal.DTOs;

namespace DashboardPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WidgetsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WidgetsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/widgets
        [HttpGet]
        public async Task<ActionResult<List<WidgetDto>>> GetAllWidgets()
        {
            var widgets = await _context.Widgets
                .Where(w => w.IsActive)
                .OrderBy(w => w.WidgetName)
                .ToListAsync();

            return Ok(widgets.Select(w => MapToDto(w)).ToList());
        }

        // GET: api/widgets/role/{roleId}
        [HttpGet("role/{roleId}")]
        public async Task<ActionResult<List<WidgetDto>>> GetWidgetsByRole(string roleId)
        {
            var widgets = await _context.RoleWidgets
                .Include(rw => rw.Widget)
                .Where(rw => rw.RoleId == roleId && rw.Widget.IsActive)
                .OrderBy(rw => rw.OrderIndex)
                .Select(rw => new WidgetDto
                {
                    WidgetId = rw.Widget.WidgetId,
                    WidgetName = rw.Widget.WidgetName,
                    WidgetUrl = rw.Widget.WidgetUrl,
                    IsActive = rw.Widget.IsActive,
                    OrderIndex = rw.OrderIndex
                })
                .ToListAsync();

            return Ok(widgets);
        }

        // GET: api/widgets/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<WidgetDto>> GetWidget(string id)
        {
            var widget = await _context.Widgets.FindAsync(id);

            if (widget == null)
                return NotFound(new { message = "Widget not found" });

            return Ok(MapToDto(widget));
        }

        // POST: api/widgets
        [HttpPost]
        public async Task<ActionResult<WidgetDto>> CreateWidget([FromBody] CreateWidgetDto dto)
        {
            var widget = new Widget
            {
                WidgetId = $"widget-{Guid.NewGuid().ToString().Substring(0, 8)}",
                WidgetName = dto.WidgetName,
                WidgetUrl = dto.WidgetUrl,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Widgets.Add(widget);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetWidget), new { id = widget.WidgetId }, MapToDto(widget));
        }

        // PUT: api/widgets/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<WidgetDto>> UpdateWidget(string id, [FromBody] CreateWidgetDto dto)
        {
            var widget = await _context.Widgets.FindAsync(id);

            if (widget == null)
                return NotFound(new { message = "Widget not found" });

            widget.WidgetName = dto.WidgetName;
            widget.WidgetUrl = dto.WidgetUrl;
            widget.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(MapToDto(widget));
        }

        // DELETE: api/widgets/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWidget(string id)
        {
            var widget = await _context.Widgets.FindAsync(id);

            if (widget == null)
                return NotFound(new { message = "Widget not found" });

            _context.Widgets.Remove(widget);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/widgets/role/{roleId}/assign
        [HttpPost("role/{roleId}/assign")]
        public async Task<IActionResult> AssignWidgetsToRole(string roleId, [FromBody] AssignWidgetsRequest request)
        {
            var role = await _context.UserRoles.FindAsync(roleId);
            if (role == null)
                return NotFound(new { message = "Role not found" });

            var maxOrder = await _context.RoleWidgets
                .Where(rw => rw.RoleId == roleId)
                .MaxAsync(rw => (int?)rw.OrderIndex) ?? -1;

            foreach (var widgetId in request.WidgetIds)
            {
                var exists = await _context.RoleWidgets
                    .AnyAsync(rw => rw.RoleId == roleId && rw.WidgetId == widgetId);

                if (!exists)
                {
                    _context.RoleWidgets.Add(new RoleWidget
                    {
                        RoleId = roleId,
                        WidgetId = widgetId,
                        OrderIndex = ++maxOrder,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Widgets assigned successfully" });
        }

        // DELETE: api/widgets/role/{roleId}/unassign/{widgetId}
        [HttpDelete("role/{roleId}/unassign/{widgetId}")]
        public async Task<IActionResult> UnassignWidgetFromRole(string roleId, string widgetId)
        {
            var roleWidget = await _context.RoleWidgets
                .FirstOrDefaultAsync(rw => rw.RoleId == roleId && rw.WidgetId == widgetId);

            if (roleWidget == null)
                return NotFound(new { message = "Assignment not found" });

            _context.RoleWidgets.Remove(roleWidget);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private WidgetDto MapToDto(Widget widget)
        {
            return new WidgetDto
            {
                WidgetId = widget.WidgetId,
                WidgetName = widget.WidgetName,
                WidgetUrl = widget.WidgetUrl,
                IsActive = widget.IsActive
            };
        }
    }

    public class AssignWidgetsRequest
    {
        public List<string> WidgetIds { get; set; }
    }
}