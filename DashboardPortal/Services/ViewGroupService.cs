using Microsoft.EntityFrameworkCore;
using DashboardPortal.Data;
using DashboardPortal.Models;
using DashboardPortal.DTOs;
using System.Text.Json;

namespace DashboardPortal.Services
{
    public class ViewGroupService : IViewGroupService
    {
        private readonly ApplicationDbContext _context;

        public ViewGroupService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ViewGroupDto>> GetUserViewGroupsAsync(string userId)
        {
            var viewGroups = await _context.ViewGroups
                .Include(vg => vg.ViewGroupViews)
                    .ThenInclude(vgv => vgv.View)
                        .ThenInclude(v => v.ViewReports)
                            .ThenInclude(vr => vr.Report)
                .Include(vg => vg.ViewGroupViews)
                    .ThenInclude(vgv => vgv.View)
                        .ThenInclude(v => v.ViewWidgets)
                            .ThenInclude(vw => vw.Widget)
                .Where(vg => vg.UserId == userId)
                .OrderBy(vg => vg.OrderIndex)
                .ToListAsync();

            return viewGroups.Select(vg => MapToDto(vg)).ToList();
        }

        public async Task<ViewGroupDto> GetViewGroupByIdAsync(string viewGroupId, string userId)
        {
            var viewGroup = await _context.ViewGroups
                .Include(vg => vg.ViewGroupViews)
                    .ThenInclude(vgv => vgv.View)
                        .ThenInclude(v => v.ViewReports)
                            .ThenInclude(vr => vr.Report)
                .Include(vg => vg.ViewGroupViews)
                    .ThenInclude(vgv => vgv.View)
                        .ThenInclude(v => v.ViewWidgets)
                            .ThenInclude(vw => vw.Widget)
                .FirstOrDefaultAsync(vg => vg.ViewGroupId == viewGroupId && vg.UserId == userId);

            if (viewGroup == null)
                return null;

            return MapToDto(viewGroup);
        }

        public async Task<ViewGroupDto> CreateViewGroupAsync(string userId, CreateViewGroupDto dto)
        {
            var viewGroup = new ViewGroup
            {
                ViewGroupId = $"vg-{userId}-{Guid.NewGuid().ToString().Substring(0, 8)}",
                UserId = userId,
                Name = dto.Name,
                IsVisible = dto.IsVisible,
                IsDefault = dto.IsDefault,
                OrderIndex = dto.OrderIndex,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.ViewGroups.Add(viewGroup);
            await _context.SaveChangesAsync();

            return MapToDto(viewGroup);
        }

        public async Task<ViewGroupDto> UpdateViewGroupAsync(string viewGroupId, string userId, UpdateViewGroupDto dto)
        {
            var viewGroup = await _context.ViewGroups
                .FirstOrDefaultAsync(vg => vg.ViewGroupId == viewGroupId && vg.UserId == userId);

            if (viewGroup == null)
                return null;

            viewGroup.Name = dto.Name;
            viewGroup.IsVisible = dto.IsVisible;
            viewGroup.IsDefault = dto.IsDefault;
            viewGroup.OrderIndex = dto.OrderIndex;
            viewGroup.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToDto(viewGroup);
        }

        public async Task<bool> DeleteViewGroupAsync(string viewGroupId, string userId)
        {
            var viewGroup = await _context.ViewGroups
                .FirstOrDefaultAsync(vg => vg.ViewGroupId == viewGroupId && vg.UserId == userId);

            if (viewGroup == null)
                return false;

            _context.ViewGroups.Remove(viewGroup);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ReorderViewGroupsAsync(string userId, List<ReorderItemDto> items)
        {
            var viewGroups = await _context.ViewGroups
                .Where(vg => vg.UserId == userId)
                .ToListAsync();

            foreach (var item in items)
            {
                var viewGroup = viewGroups.FirstOrDefault(vg => vg.ViewGroupId == item.Id);
                if (viewGroup != null)
                {
                    viewGroup.OrderIndex = item.OrderIndex;
                    viewGroup.UpdatedAt = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddViewsToGroupAsync(string viewGroupId, string userId, List<string> viewIds)
        {
            var viewGroup = await _context.ViewGroups
                .FirstOrDefaultAsync(vg => vg.ViewGroupId == viewGroupId && vg.UserId == userId);

            if (viewGroup == null)
                return false;

            var maxOrder = await _context.ViewGroupViews
                .Where(vgv => vgv.ViewGroupId == viewGroupId)
                .MaxAsync(vgv => (int?)vgv.OrderIndex) ?? 0;

            foreach (var viewId in viewIds)
            {
                var view = await _context.Views
                    .FirstOrDefaultAsync(v => v.ViewId == viewId && v.UserId == userId);

                if (view == null)
                    continue;

                var exists = await _context.ViewGroupViews
                    .AnyAsync(vgv => vgv.ViewGroupId == viewGroupId && vgv.ViewId == viewId);

                if (!exists)
                {
                    _context.ViewGroupViews.Add(new ViewGroupView
                    {
                        ViewGroupId = viewGroupId,
                        ViewId = viewId,
                        OrderIndex = ++maxOrder,
                        CreatedBy = userId,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveViewFromGroupAsync(string viewGroupId, string viewId, string userId)
        {
            var viewGroup = await _context.ViewGroups
                .FirstOrDefaultAsync(vg => vg.ViewGroupId == viewGroupId && vg.UserId == userId);

            if (viewGroup == null)
                return false;

            var viewGroupView = await _context.ViewGroupViews
                .FirstOrDefaultAsync(vgv => vgv.ViewGroupId == viewGroupId && vgv.ViewId == viewId);

            if (viewGroupView == null)
                return false;

            _context.ViewGroupViews.Remove(viewGroupView);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ReorderViewsInGroupAsync(string viewGroupId, string userId, List<ReorderItemDto> items)
        {
            var viewGroup = await _context.ViewGroups
                .FirstOrDefaultAsync(vg => vg.ViewGroupId == viewGroupId && vg.UserId == userId);

            if (viewGroup == null)
                return false;

            var viewGroupViews = await _context.ViewGroupViews
                .Where(vgv => vgv.ViewGroupId == viewGroupId)
                .ToListAsync();

            foreach (var item in items)
            {
                var vgv = viewGroupViews.FirstOrDefault(vgv => vgv.ViewId == item.Id);
                if (vgv != null)
                {
                    vgv.OrderIndex = item.OrderIndex;
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        private ViewGroupDto MapToDto(ViewGroup viewGroup)
        {
            return new ViewGroupDto
            {
                ViewGroupId = viewGroup.ViewGroupId,
                UserId = viewGroup.UserId,
                Name = viewGroup.Name,
                IsVisible = viewGroup.IsVisible,
                IsDefault = viewGroup.IsDefault,
                OrderIndex = viewGroup.OrderIndex,
                CreatedBy = viewGroup.CreatedBy,
                CreatedAt = viewGroup.CreatedAt,
                UpdatedAt = viewGroup.UpdatedAt,
                Views = viewGroup.ViewGroupViews?
                    .OrderBy(vgv => vgv.OrderIndex)
                    .Select(vgv => new ViewDto
                    {
                        ViewId = vgv.View.ViewId,
                        UserId = vgv.View.UserId,
                        Name = vgv.View.Name,
                        IsVisible = vgv.View.IsVisible,
                        OrderIndex = vgv.View.OrderIndex,
                        CreatedBy = vgv.View.CreatedBy,
                        CreatedAt = vgv.View.CreatedAt,
                        UpdatedAt = vgv.View.UpdatedAt,
                        Reports = vgv.View.ViewReports?
                            .OrderBy(vr => vr.OrderIndex)
                            .Select(vr => new ReportDto
                            {
                                ReportId = vr.Report.ReportId,
                                ReportName = vr.Report.ReportName,
                                ReportUrl = vr.Report.ReportUrl,
                                IsActive = vr.Report.IsActive,
                                OrderIndex = vr.OrderIndex
                            }).ToList(),
                        Widgets = vgv.View.ViewWidgets?
                            .OrderBy(vw => vw.OrderIndex)
                            .Select(vw => new WidgetDto
                            {
                                WidgetId = vw.Widget.WidgetId,
                                WidgetName = vw.Widget.WidgetName,
                                WidgetUrl = vw.Widget.WidgetUrl,
                                IsActive = vw.Widget.IsActive,
                                OrderIndex = vw.OrderIndex
                            }).ToList()
                    }).ToList() ?? new List<ViewDto>()
            };
        }
    }
}