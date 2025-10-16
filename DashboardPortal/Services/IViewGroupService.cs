using DashboardPortal.DTOs;

namespace DashboardPortal.Services
{
    public interface IViewGroupService
    {
        Task<List<ViewGroupDto>> GetUserViewGroupsAsync(string userId);
        Task<ViewGroupDto> GetViewGroupByIdAsync(string viewGroupId, string userId);
        Task<ViewGroupDto> CreateViewGroupAsync(string userId, CreateViewGroupDto dto);
        Task<ViewGroupDto> UpdateViewGroupAsync(string viewGroupId, string userId, UpdateViewGroupDto dto);
        Task<bool> DeleteViewGroupAsync(string viewGroupId, string userId);
        Task<bool> ReorderViewGroupsAsync(string userId, List<ReorderItemDto> items);
        Task<bool> AddViewsToGroupAsync(string viewGroupId, string userId, List<string> viewIds);
        Task<bool> RemoveViewFromGroupAsync(string viewGroupId, string viewId, string userId);
        Task<bool> ReorderViewsInGroupAsync(string viewGroupId, string userId, List<ReorderItemDto> items);
    }
}
