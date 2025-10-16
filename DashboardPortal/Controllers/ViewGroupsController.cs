using Microsoft.AspNetCore.Mvc;
using DashboardPortal.Services;
using DashboardPortal.DTOs;

namespace DashboardPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ViewGroupsController : ControllerBase
    {
        private readonly IViewGroupService _viewGroupService;

        public ViewGroupsController(IViewGroupService viewGroupService)
        {
            _viewGroupService = viewGroupService;
        }

        // GET: api/viewgroups/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<ViewGroupDto>>> GetUserViewGroups(string userId)
        {
            var viewGroups = await _viewGroupService.GetUserViewGroupsAsync(userId);
            return Ok(viewGroups);
        }

        // GET: api/viewgroups/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ViewGroupDto>> GetViewGroup(string id, [FromQuery] string userId)
        {
            var viewGroup = await _viewGroupService.GetViewGroupByIdAsync(id, userId);

            if (viewGroup == null)
                return NotFound(new { message = "ViewGroup not found" });

            return Ok(viewGroup);
        }

        // POST: api/viewgroups
        [HttpPost]
        public async Task<ActionResult<ViewGroupDto>> CreateViewGroup([FromBody] CreateViewGroupRequest request)
        {
            var viewGroup = await _viewGroupService.CreateViewGroupAsync(request.UserId, request.Data);
            return CreatedAtAction(nameof(GetViewGroup), new { id = viewGroup.ViewGroupId, userId = request.UserId }, viewGroup);
        }

        // PUT: api/viewgroups/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<ViewGroupDto>> UpdateViewGroup(string id, [FromBody] UpdateViewGroupRequest request)
        {
            var viewGroup = await _viewGroupService.UpdateViewGroupAsync(id, request.UserId, request.Data);

            if (viewGroup == null)
                return NotFound(new { message = "ViewGroup not found" });

            return Ok(viewGroup);
        }

        // DELETE: api/viewgroups/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteViewGroup(string id, [FromQuery] string userId)
        {
            var result = await _viewGroupService.DeleteViewGroupAsync(id, userId);

            if (!result)
                return NotFound(new { message = "ViewGroup not found" });

            return NoContent();
        }

        // POST: api/viewgroups/reorder
        [HttpPost("reorder")]
        public async Task<IActionResult> ReorderViewGroups([FromBody] ReorderViewGroupsRequest request)
        {
            var result = await _viewGroupService.ReorderViewGroupsAsync(request.UserId, request.Items);

            if (!result)
                return BadRequest(new { message = "Failed to reorder view groups" });

            return Ok(new { message = "View groups reordered successfully" });
        }

        // POST: api/viewgroups/{id}/views
        [HttpPost("{id}/views")]
        public async Task<IActionResult> AddViewsToGroup(string id, [FromBody] AddViewsToGroupRequest request)
        {
            var result = await _viewGroupService.AddViewsToGroupAsync(id, request.UserId, request.ViewIds);

            if (!result)
                return NotFound(new { message = "ViewGroup not found" });

            return Ok(new { message = "Views added successfully" });
        }

        // DELETE: api/viewgroups/{viewGroupId}/views/{viewId}
        [HttpDelete("{viewGroupId}/views/{viewId}")]
        public async Task<IActionResult> RemoveViewFromGroup(string viewGroupId, string viewId, [FromQuery] string userId)
        {
            var result = await _viewGroupService.RemoveViewFromGroupAsync(viewGroupId, viewId, userId);

            if (!result)
                return NotFound(new { message = "View or ViewGroup not found" });

            return NoContent();
        }

        // POST: api/viewgroups/{id}/views/reorder
        [HttpPost("{id}/views/reorder")]
        public async Task<IActionResult> ReorderViewsInGroup(string id, [FromBody] ReorderViewsRequest request)
        {
            var result = await _viewGroupService.ReorderViewsInGroupAsync(id, request.UserId, request.Items);

            if (!result)
                return NotFound(new { message = "ViewGroup not found" });

            return Ok(new { message = "Views reordered successfully" });
        }
    }

    // Request DTOs
    public class CreateViewGroupRequest
    {
        public string UserId { get; set; }
        public CreateViewGroupDto Data { get; set; }
    }

    public class UpdateViewGroupRequest
    {
        public string UserId { get; set; }
        public UpdateViewGroupDto Data { get; set; }
    }

    public class ReorderViewGroupsRequest
    {
        public string UserId { get; set; }
        public List<ReorderItemDto> Items { get; set; }
    }

    public class AddViewsToGroupRequest
    {
        public string UserId { get; set; }
        public List<string> ViewIds { get; set; }
    }

    public class ReorderViewsRequest
    {
        public string UserId { get; set; }
        public List<ReorderItemDto> Items { get; set; }
    }
}