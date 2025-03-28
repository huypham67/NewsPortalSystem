using Microsoft.AspNetCore.SignalR;

namespace RealTimeHub
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"User connected: {Context.UserIdentifier}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"User disconnected: {Context.UserIdentifier}");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendNotification(string userId, string message, string articleId)
        {
            Console.WriteLine($"Sending notification to user {userId}: {message}");
            if (!string.IsNullOrEmpty(userId))
            {
                await Clients.User(userId).SendAsync("ReceiveNotification", message, articleId);
            }
        }
    }
}
