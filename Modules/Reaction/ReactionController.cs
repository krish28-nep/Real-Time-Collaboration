using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RealTimeCollaboration.Modules.Auth.Utils;
using RealTimeCollaboration.Modules.Message.Interfaces;
using RealTimeCollaboration.Modules.Reaction.DTOs;
using RealTimeCollaboration.Modules.Reaction.Interfaces;
using RealTimeCollaboration.Modules.SignalR;

namespace RealTimeCollaboration.Modules.Reaction;

[ApiController]
[Authorize]
[Route("api/messages/{messageId:int}/reactions")]
public class ReactionController : ControllerBase
{
    private readonly IReactionService _reactionService;
    private readonly IMessageRepository _messageRepository;
    private readonly IHubContext<ChatHub> _hubContext;

    public ReactionController(
        IReactionService reactionService,
        IMessageRepository messageRepository,
        IHubContext<ChatHub> hubContext)
    {
        _reactionService = reactionService;
        _messageRepository = messageRepository;
        _hubContext = hubContext;
    }

    [HttpPost]
    public async Task<ActionResult<ReactionResponseDTO>> CreateReaction(
        int messageId,
        [FromBody] CreateReactionDTO createReactionDTO)
    {
        var userId = AuthUserContext.GetCurrentUserId(User);
        if (userId is null)
        {
            return Unauthorized();
        }

        try
        {
            var reaction = await _reactionService.CreateAsync(messageId, userId.Value, createReactionDTO);
            var message = await _messageRepository.GetByIdAsync(messageId);
            if (message is not null)
            {
                await _hubContext.Clients
                    .Group(ChatHub.GetChannelGroupName(message.ChannelId))
                    .SendAsync("reaction.created", reaction);
            }

            return CreatedAtAction(nameof(CreateReaction), new { messageId }, reaction);
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

    [HttpDelete("{emoji}")]
    public async Task<IActionResult> DeleteReaction(int messageId, string emoji)
    {
        var userId = AuthUserContext.GetCurrentUserId(User);
        if (userId is null)
        {
            return Unauthorized();
        }

        var deleted = await _reactionService.DeleteAsync(messageId, userId.Value, emoji);
        if (!deleted)
        {
            return NotFound();
        }

        var message = await _messageRepository.GetByIdAsync(messageId);
        if (message is not null)
        {
            await _hubContext.Clients
                .Group(ChatHub.GetChannelGroupName(message.ChannelId))
                .SendAsync("reaction.deleted", new { messageId, userId = userId.Value, emoji });
        }

        return NoContent();
    }

    [HttpPatch]
    public async Task<ActionResult<ReactionResponseDTO>> UpdateReaction(
        int messageId,
        [FromBody] UpdateReactionDTO updateReactionDTO)
    {
        var userId = AuthUserContext.GetCurrentUserId(User);
        if (userId is null)
        {
            return Unauthorized();
        }

        try
        {
            var reaction = await _reactionService.UpdateAsync(messageId, userId.Value, updateReactionDTO);
            if (reaction is null)
            {
                return NotFound();
            }

            var message = await _messageRepository.GetByIdAsync(messageId);
            if (message is not null)
            {
                await _hubContext.Clients
                    .Group(ChatHub.GetChannelGroupName(message.ChannelId))
                    .SendAsync("reaction.updated", reaction);
            }

            return Ok(reaction);
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
}
