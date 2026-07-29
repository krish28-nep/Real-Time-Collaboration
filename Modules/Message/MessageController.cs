using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc;
using RealTimeCollaboration.Modules.Auth.Utils;
using RealTimeCollaboration.Modules.Message.DTOs;
using RealTimeCollaboration.Modules.Message.Interfaces;
using RealTimeCollaboration.Modules.SignalR;

namespace RealTimeCollaboration.Modules.Message;

[ApiController]
[Authorize]
[Route("api/channels/{channelId:int}/messages")]
public class MessageController : ControllerBase
{
    private readonly IMessageService _messageService;
    private readonly IHubContext<ChatHub> _hubContext;

    public MessageController(IMessageService messageService, IHubContext<ChatHub> hubContext)
    {
        _messageService = messageService;
        _hubContext = hubContext;
    }

    [HttpPost]
    public async Task<ActionResult<MessageResponseDTO>> CreateMessage(
        int channelId,
        [FromBody] CreateMessageDTO createMessageDTO)
    {
        var userId = AuthUserContext.GetCurrentUserId(User);
        if (userId is null)
        {
            return Unauthorized();
        }

        try
        {
            var message = await _messageService.CreateAsync(channelId, userId.Value, createMessageDTO);
            await _hubContext.Clients
                .Group(ChatHub.GetChannelGroupName(channelId))
                .SendAsync("message.created", message);

            return CreatedAtAction(nameof(GetMessages), new { channelId }, message);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<MessageListResponseDTO>> GetMessages(
        int channelId,
        [FromQuery] MessagePaginationDTO paginationDTO)
    {
        var userId = AuthUserContext.GetCurrentUserId(User);
        if (userId is null)
        {
            return Unauthorized();
        }

        var messages = await _messageService.GetByChannelIdAsync(channelId, userId.Value, paginationDTO);

        return Ok(messages);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteMessage(int channelId, int id)
    {
        var userId = AuthUserContext.GetCurrentUserId(User);
        if (userId is null)
        {
            return Unauthorized();
        }

        var deleted = await _messageService.DeleteAsync(id, channelId, userId.Value);
        if (!deleted)
        {
            return NotFound();
        }

        await _hubContext.Clients
            .Group(ChatHub.GetChannelGroupName(channelId))
            .SendAsync("message.deleted", new { id, channelId });

        return NoContent();
    }
}
