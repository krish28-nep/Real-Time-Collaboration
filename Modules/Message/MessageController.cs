using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc;
using RealTimeCollaboration.Modules.Auth.Utils;
using RealTimeCollaboration.Modules.Message.DTOs;
using RealTimeCollaboration.Modules.Message.Interfaces;
using RealTimeCollaboration.Modules.SignalR;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace RealTimeCollaboration.Modules.Message;

[ApiController]
[Authorize]
[Route("api/channels/{channelId:int}/messages")]
public class MessageController : ControllerBase
{
    private const int MaxImagesPerMessage = 4;
    private const long MaxImageSizeInBytes = 5 * 1024 * 1024;
    private const int MaxOptimizedImageDimension = 1600;
    private const int OptimizedWebpQuality = 82;
    private static readonly string[] AllowedImageExtensions = [".jpg", ".jpeg", ".png", ".webp", ".gif"];
    private static readonly string[] AllowedImageContentTypes = ["image/jpeg", "image/png", "image/webp", "image/gif"];

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

    [HttpPost("images")]
    public async Task<IActionResult> UploadMessageImages(int channelId, [FromForm] List<IFormFile> images)
    {
        var userId = AuthUserContext.GetCurrentUserId(User);
        if (userId is null)
        {
            return Unauthorized();
        }

        if (images.Count == 0)
        {
            return BadRequest(new { message = "Choose at least one image." });
        }

        if (images.Count > MaxImagesPerMessage)
        {
            return BadRequest(new { message = $"You can upload up to {MaxImagesPerMessage} images per message." });
        }

        try
        {
            var urls = new List<string>();
            var folderPath = Path.Combine("wwwroot", "message-images");
            Directory.CreateDirectory(folderPath);

            foreach (var image in images)
            {
                ValidateMessageImage(image);

                var fileName = $"channel-{channelId}-user-{userId}-{Guid.NewGuid()}.webp";
                var fullPath = Path.Combine(folderPath, fileName);

                await SaveOptimizedImageAsync(image, fullPath);

                urls.Add($"/message-images/{fileName}");
            }

            return Ok(new { images = urls });
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

    private static void ValidateMessageImage(IFormFile image)
    {
        if (image.Length == 0)
        {
            throw new ArgumentException("Image file is empty. Please upload a valid image.");
        }

        if (image.Length > MaxImageSizeInBytes)
        {
            throw new ArgumentException("Image file is too large. Please upload an image smaller than 5 MB.");
        }

        var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
        var isSupportedExtension = AllowedImageExtensions.Contains(extension);
        var isSupportedContentType = AllowedImageContentTypes.Contains(image.ContentType);

        if (!isSupportedExtension || !isSupportedContentType)
        {
            throw new ArgumentException("Image file type is not supported. Please upload a JPG, PNG, WEBP, or GIF image.");
        }
    }

    private static async Task SaveOptimizedImageAsync(IFormFile image, string fullPath)
    {
        await using var inputStream = image.OpenReadStream();
        using var optimizedImage = await Image.LoadAsync(inputStream);

        var largestDimension = Math.Max(optimizedImage.Width, optimizedImage.Height);
        if (largestDimension > MaxOptimizedImageDimension)
        {
            optimizedImage.Mutate(operation => operation.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(MaxOptimizedImageDimension, MaxOptimizedImageDimension)
            }));
        }

        var encoder = new WebpEncoder
        {
            Quality = OptimizedWebpQuality
        };

        await optimizedImage.SaveAsWebpAsync(fullPath, encoder);
    }
}
