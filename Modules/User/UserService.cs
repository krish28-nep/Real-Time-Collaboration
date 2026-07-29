using Microsoft.AspNetCore.Identity;
using RealTimeCollaboration.Modules.User.DTOs;
using RealTimeCollaboration.Modules.User.Interfaces;

namespace RealTimeCollaboration.Modules.User;

public class UserService : IUserService
{
    private const long MaxAvatarSizeInBytes = 2 * 1024 * 1024;
    private static readonly string[] AllowedAvatarExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private static readonly string[] AllowedAvatarContentTypes = ["image/jpeg", "image/png", "image/webp"];

    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<Models.User> _passwordHasher;

    public UserService(
        IUserRepository userRepository,
        IPasswordHasher<Models.User> passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<IEnumerable<Models.User>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    public async Task<Models.User?> GetUserByIdAsync(int id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    public async Task<Models.User?> GetUserByUsernameAsync(string username)
    {
        return await _userRepository.GetByUsernameAsync(username);
    }

    public async Task<Models.User> CreateUserAsync(CreateUserDTO createUserDto)
    {
        var existingUser = await _userRepository.GetByEmailAsync(createUserDto.Email);
        if (existingUser is not null)
        {
            throw new InvalidOperationException("A user with this email already exists.");
        }

        string? savedAvatarUrl = null;
        if (createUserDto.AvatarUrl is not null)
        {
            ValidateAvatar(createUserDto.AvatarUrl);

            var folderPath = Path.Combine("wwwroot", "avatars");
            Directory.CreateDirectory(folderPath);

            var safeUsername = string.Concat(createUserDto.Username.Split(Path.GetInvalidFileNameChars()));
            safeUsername = safeUsername.Replace(" ", "_");

            var extension = Path.GetExtension(createUserDto.AvatarUrl.FileName);
            var fileName = $"{safeUsername}_{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(folderPath, fileName);

            await using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await createUserDto.AvatarUrl.CopyToAsync(stream);
            }
            savedAvatarUrl = $"/avatars/{fileName}";
        }

        var user = new Models.User
        {
            Username = createUserDto.Username,
            Email = createUserDto.Email,
            PasswordHash = string.Empty,
            AvatarUrl = savedAvatarUrl,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, createUserDto.Password);

        return await _userRepository.CreateAsync(user);
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        return await _userRepository.DeleteAsync(id);
    }

    private static void ValidateAvatar(IFormFile avatar)
    {
        if (avatar.Length == 0)
        {
            throw new ArgumentException("Avatar file is empty. Please upload a valid image.");
        }

        if (avatar.Length > MaxAvatarSizeInBytes)
        {
            throw new ArgumentException("Avatar file is too large. Please upload an image smaller than 2 MB.");
        }

        var extension = Path.GetExtension(avatar.FileName).ToLowerInvariant();
        var isSupportedExtension = AllowedAvatarExtensions.Contains(extension);
        var isSupportedContentType = AllowedAvatarContentTypes.Contains(avatar.ContentType);

        if (!isSupportedExtension || !isSupportedContentType)
        {
            throw new ArgumentException("Avatar file type is not supported. Please upload a JPG, PNG, or WEBP image.");
        }
    }
}
