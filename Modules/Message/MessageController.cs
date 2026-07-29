using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealTimeCollaboration.Modules.Auth.Utils;
using RealTimeCollaboration.Modules.Message.DTOs;
using RealTimeCollaboration.Modules.Message.Interfaces;

namespace RealTimeCollaboration.Modules.Message;

[ApiController]
[Authorize]
[Route("api/channels/{channelId:int}/messages")]
public class MessageController : ControllerBase
{
    private readonly IMessageService _messageService;

    public MessageController(IMessageService messageService)
    {
        _messageService = messageService;
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

        return NoContent();
    }
}
