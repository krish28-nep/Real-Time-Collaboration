using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealTimeCollaboration.Modules.Auth.Utils;
using RealTimeCollaboration.Modules.WorkSpace.DTOs;
using RealTimeCollaboration.Modules.WorkSpace.Interfaces;

namespace RealTimeCollaboration.Modules.WorkSpace;

[ApiController]
[Authorize]
[Route("api/workspaces")]
public class WorkSpaceController : ControllerBase
{
    private readonly IWorkSpaceService _workSpaceService;

    public WorkSpaceController(IWorkSpaceService workSpaceService)
    {
        _workSpaceService = workSpaceService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkSpaceResponseDTO>>> GetMyWorkSpaces()
    {
        var userId = AuthUserContext.GetCurrentUserId(User);
        if (userId is null)
        {
            return Unauthorized();
        }

        var workSpaces = await _workSpaceService.GetAllByUserIdAsync(userId.Value);

        return Ok(workSpaces);
    }

    [HttpGet("{identifier}")]
    public async Task<ActionResult<WorkSpaceResponseDTO>> GetWorkSpaceById(string identifier)
    {
        var userId = AuthUserContext.GetCurrentUserId(User);
        if (userId is null)
        {
            return Unauthorized();
        }

        var workSpace = await _workSpaceService.GetByIdentifierAsync(identifier, userId.Value);
        if (workSpace is null)
        {
            return NotFound();
        }

        return Ok(workSpace);
    }

    [HttpGet("{workspaceId}/users")]
    public async Task<ActionResult<IEnumerable<WorkSpaceUserDTO>>> GetUsersByWorkSpaceId(int workspaceId)
    {
        var userId = AuthUserContext.GetCurrentUserId(User);
        if (userId is null)
        {
            return Unauthorized();
        }
        var users = await _workSpaceService.GetAllUserByWorkspaceIdAsync(workspaceId);
        return Ok(users);
    }

    [HttpPost]
    public async Task<ActionResult<WorkSpaceResponseDTO>> CreateWorkSpace([FromBody] CreateWorkSpaceDTO createWorkSpaceDTO)
    {
        var userId = AuthUserContext.GetCurrentUserId(User);
        if (userId is null)
        {
            return Unauthorized();
        }

        try
        {
            var workSpace = await _workSpaceService.CreateAsync(createWorkSpaceDTO, userId.Value);
            return CreatedAtAction(nameof(GetWorkSpaceById), new { identifier = workSpace.Slug }, workSpace);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteWorkSpace(int id)
    {
        var userId = AuthUserContext.GetCurrentUserId(User);
        if (userId is null)
        {
            return Unauthorized();
        }

        var deleted = await _workSpaceService.DeleteAsync(id, userId.Value);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
