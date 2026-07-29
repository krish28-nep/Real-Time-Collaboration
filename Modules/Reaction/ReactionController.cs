using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealTimeCollaboration.Modules.Auth.Utils;
using RealTimeCollaboration.Modules.Reaction.DTOs;
using RealTimeCollaboration.Modules.Reaction.Interfaces;

namespace RealTimeCollaboration.Modules.Reaction;

[ApiController]
[Authorize]
[Route("api/messages/{messageId:int}/reactions")]
public class ReactionController : ControllerBase
{
    private readonly IReactionService _reactionService;

    public ReactionController(IReactionService reactionService)
    {
        _reactionService = reactionService;
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
