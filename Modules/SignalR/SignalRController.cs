using Microsoft.AspNetCore.SignalR;

namespace RealTimeCollaboration.Modules.SignalR;

public class ChatHub : Hub
{
    public async Task JoinUser(int userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, GetUserGroupName(userId));
        await Clients.Caller.SendAsync("JoinedUser", userId);
    }

    public async Task SendPrivateMessage(int senderId, int receiverId, string message)
    {
        var sentAt = DateTime.UtcNow;

        await Clients.Group(GetUserGroupName(receiverId)).SendAsync(
            "ReceivePrivateMessage",
            senderId,
            receiverId,
            message,
            sentAt);

        await Clients.Caller.SendAsync(
            "PrivateMessageSent",
            senderId,
            receiverId,
            message,
            sentAt);
    }

    private static string GetUserGroupName(int userId)
    {
        return $"user:{userId}";
    }
}
