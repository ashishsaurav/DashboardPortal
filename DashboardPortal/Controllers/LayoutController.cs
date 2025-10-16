using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DashboardPortal.Data;
using DashboardPortal.Models;
using DashboardPortal.DTOs;

namespace DashboardPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LayoutController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LayoutController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/layout/{userId}
        [HttpGet("{userId}")]
        public async Task<ActionResult<List<LayoutCustomizationDto>>> GetUserLayouts(string userId)
        {
            var layouts = await _context.LayoutCustomizations
                .Where(lc => lc.UserId == userId)
                .OrderByDescending(lc => lc.UpdatedAt)
                .ToListAsync();

            return Ok(layouts.Select(l => MapToDto(l)).ToList());
        }

        // GET: api/layout/{userId}/{signature}
        [HttpGet("{userId}/{signature}")]
        public async Task<ActionResult<LayoutCustomizationDto>> GetLayout(string userId, string signature)
        {
            var layout = await _context.LayoutCustomizations
                .FirstOrDefaultAsync(lc => lc.UserId == userId && lc.LayoutSignature == signature);

            if (layout == null)
                return NotFound(new { message = "Layout not found" });

            return Ok(MapToDto(layout));
        }

        // POST: api/layout/{userId}
        [HttpPost("{userId}")]
        public async Task<ActionResult<LayoutCustomizationDto>> SaveLayout(
            string userId,
            [FromBody] SaveLayoutDto dto)
        {
            var existingLayout = await _context.LayoutCustomizations
                .FirstOrDefaultAsync(lc => lc.UserId == userId && lc.LayoutSignature == dto.LayoutSignature);

            if (existingLayout != null)
            {
                // Update existing layout
                existingLayout.LayoutData = dto.LayoutData;
                existingLayout.Timestamp = dto.Timestamp;
                existingLayout.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                // Create new layout
                existingLayout = new LayoutCustomization
                {
                    UserId = userId,
                    LayoutSignature = dto.LayoutSignature,
                    LayoutData = dto.LayoutData,
                    Timestamp = dto.Timestamp,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.LayoutCustomizations.Add(existingLayout);
            }

            await _context.SaveChangesAsync();

            return Ok(MapToDto(existingLayout));
        }

        // DELETE: api/layout/{userId}/{signature}
        [HttpDelete("{userId}/{signature}")]
        public async Task<IActionResult> DeleteLayout(string userId, string signature)
        {
            var layout = await _context.LayoutCustomizations
                .FirstOrDefaultAsync(lc => lc.UserId == userId && lc.LayoutSignature == signature);

            if (layout == null)
                return NotFound(new { message = "Layout not found" });

            _context.LayoutCustomizations.Remove(layout);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/layout/{userId}
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteAllUserLayouts(string userId)
        {
            var layouts = await _context.LayoutCustomizations
                .Where(lc => lc.UserId == userId)
                .ToListAsync();

            _context.LayoutCustomizations.RemoveRange(layouts);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private LayoutCustomizationDto MapToDto(LayoutCustomization layout)
        {
            return new LayoutCustomizationDto
            {
                Id = layout.Id,
                UserId = layout.UserId,
                LayoutSignature = layout.LayoutSignature,
                LayoutData = layout.LayoutData,
                Timestamp = layout.Timestamp
            };
        }
    }
}