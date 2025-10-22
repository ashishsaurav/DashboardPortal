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
    public class NavigationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NavigationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/navigation/{userId}
        [HttpGet("{userId}")]
        public async Task<ActionResult<NavigationSettingDto>> GetNavigationSettings(string userId)
        {
            var setting = await _context.NavigationSettings
                .FirstOrDefaultAsync(ns => ns.UserId == userId);

            if (setting == null)
            {
                // Create default navigation settings
                setting = new NavigationSetting
                {
                    UserId = userId,
                    ViewGroupOrder = "[]",
                    ViewOrders = "{}",
                    HiddenViewGroups = "[]",
                    HiddenViews = "[]",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.NavigationSettings.Add(setting);
                await _context.SaveChangesAsync();
            }

            return Ok(MapToDto(setting));
        }

        // PUT: api/navigation/{userId}
        [HttpPut("{userId}")]
        public async Task<ActionResult<NavigationSettingDto>> UpdateNavigationSettings(
            string userId,
            [FromBody] UpdateNavigationSettingDto dto)
        {
            var setting = await _context.NavigationSettings
                .FirstOrDefaultAsync(ns => ns.UserId == userId);

            if (setting == null)
            {
                setting = new NavigationSetting
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };
                _context.NavigationSettings.Add(setting);
            }

            setting.ViewGroupOrder = JsonSerializer.Serialize(dto.ViewGroupOrder);
            setting.ViewOrders = JsonSerializer.Serialize(dto.ViewOrders);
            setting.HiddenViewGroups = JsonSerializer.Serialize(dto.HiddenViewGroups);
            setting.HiddenViews = JsonSerializer.Serialize(dto.HiddenViews);
            setting.UpdatedAt = DateTime.UtcNow;
            setting.ExpandedViewGroups = JsonSerializer.Serialize(dto.ExpandedViewGroups ?? new List<string>());
            setting.IsNavigationCollapsed = dto.IsNavigationCollapsed ?? false;

            await _context.SaveChangesAsync();

            return Ok(MapToDto(setting));
        }

        // DELETE: api/navigation/{userId}
        [HttpDelete("{userId}")]
        public async Task<IActionResult> ResetNavigationSettings(string userId)
        {
            var setting = await _context.NavigationSettings
                .FirstOrDefaultAsync(ns => ns.UserId == userId);

            if (setting != null)
            {
                _context.NavigationSettings.Remove(setting);
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }

        private NavigationSettingDto MapToDto(NavigationSetting setting)
        {
            return new NavigationSettingDto
            {
                Id = setting.Id,
                UserId = setting.UserId,
                ViewGroupOrder = JsonSerializer.Deserialize<List<string>>(setting.ViewGroupOrder ?? "[]"),
                ViewOrders = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(setting.ViewOrders ?? "{}"),
                HiddenViewGroups = JsonSerializer.Deserialize<List<string>>(setting.HiddenViewGroups ?? "[]"),
                HiddenViews = JsonSerializer.Deserialize<List<string>>(setting.HiddenViews ?? "[]"),
                ExpandedViewGroups = JsonSerializer.Deserialize<List<string>>(setting.ExpandedViewGroups ?? "[]"),
                IsNavigationCollapsed = setting.IsNavigationCollapsed ?? false
            };
        }
    }
}