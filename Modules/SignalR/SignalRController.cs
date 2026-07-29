using Microsoft.AspNetCore.SignalR;

namespace RealTimeCollaboration.Modules.SignalR;

public class ChatHub : Hub
{
    public async Task JoinUser(int userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, GetUserGroupName(userId));
        await Clients.Caller.SendAsync("JoinedUser", userId);
    }

    public async Task JoinChannel(int channelId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, GetChannelGroupName(channelId));
        await Clients.Caller.SendAsync("JoinedChannel", channelId);
    }

    public async Task LeaveChannel(int channelId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetChannelGroupName(channelId));
        await Clients.Caller.SendAsync("LeftChannel", channelId);
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

    public static string GetChannelGroupName(int channelId)
    {
        return $"channel:{channelId}";
    }
}
