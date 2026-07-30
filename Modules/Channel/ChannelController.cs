using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealTimeCollaboration.Modules.Auth.Utils;
using RealTimeCollaboration.Modules.Channel.DTOs;
using RealTimeCollaboration.Modules.Channel.Interfaces;

namespace RealTimeCollaboration.Modules.Channel;

[ApiController]
[Authorize]
[Route("api/workspaces/{workspaceId:int}/channels")]
public class ChannelController : ControllerBase
{
    private readonly IChannelService _channelService;

    public ChannelController(IChannelService channelService)
    {
        _channelService = channelService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Models.Channel>>> GetChannelsByWorkSpaceId(int workspaceId)
    {
        var userId = AuthUserContext.GetCurrentUserId(User);
        if (userId is null)
        {
            return Unauthorized();
        }

        var channels = await _channelService.GetAllByWorkSpaceIdAsync(workspaceId);

        return Ok(channels);
    }

    [HttpPost]
    public async Task<ActionResult<Models.Channel>> CreateChannel(int workspaceId, [FromBody] CreateChannelDTO createChannelDTO)
    {
        var userId = AuthUserContext.GetCurrentUserId(User);
        if (userId is null)
        {
            return Unauthorized();
        }

        try
        {
            var channel = await _channelService.CreateAsync(createChannelDTO, workspaceId);
            return CreatedAtAction(nameof(GetChannelsByWorkSpaceId), new { workspaceId }, channel);
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

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Models.Channel>> UpdateChannel(int workspaceId, int id, [FromBody] UpdateChannelDTO updateChannelDTO)
    {
        var userId = AuthUserContext.GetCurrentUserId(User);
        if (userId is null)
        {
            return Unauthorized();
        }

        try
        {
            var channel = await _channelService.UpdateAsync(id, workspaceId, updateChannelDTO);
            if (channel is null)
            {
                return NotFound();
            }

            return Ok(channel);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteChannel(int workspaceId, int id)
    {
        var userId = AuthUserContext.GetCurrentUserId(User);
        if (userId is null)
        {
            return Unauthorized();
        }

        var deleted = await _channelService.DeleteAsync(id, workspaceId);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
