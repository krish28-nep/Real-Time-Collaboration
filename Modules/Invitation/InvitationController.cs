using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealTimeCollaboration.Modules.Invitation.DTOs;
using RealTimeCollaboration.Modules.Invitation.Interfaces;
using RealTimeCollaboration.Modules.Auth.Utils;

namespace RealTimeCollaboration.Modules.Invitation;

[ApiController]
[Route("api/invitations")]
[Authorize]
public class InvitationController : ControllerBase
{
	private readonly IInvitationService _invitationService;

	public InvitationController(IInvitationService invitationService)
	{
		_invitationService = invitationService;
	}

	[HttpPost]
	public async Task<ActionResult<Models.Invitation>> Create([FromBody] CreateInvitationDTO dto)
	{
		var userId = AuthUserContext.GetCurrentUserId(User);
		if (userId is null) return Unauthorized();

		var invitation = await _invitationService.CreateAsync(dto.WorkSpaceId, userId.Value);
		return CreatedAtAction(nameof(GetByToken), new { token = invitation.Token }, invitation);
	}

	[AllowAnonymous]
	[HttpGet("{token}")]
	public async Task<ActionResult<Models.Invitation>> GetByToken(string token)
	{
		var inv = await _invitationService.GetByTokenAsync(token);
		if (inv is null) return NotFound();
		return Ok(inv);
	}

	[HttpPost("join/{token}")]
	public async Task<IActionResult> Join(string token)
	{
		var userId = AuthUserContext.GetCurrentUserId(User);
		if (userId is null) return Unauthorized();

		var (success, error) = await _invitationService.ConsumeAsync(token, userId.Value);
		if (!success) return BadRequest(new { message = error });

		return Ok(new { message = "Joined workspace successfully" });
	}
}
