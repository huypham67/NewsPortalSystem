using Microsoft.AspNetCore.SignalR;

namespace RealTimeHub
{
    public class NotificationHub : Hub
    {
        public async Task SendNotification(string userId, string message)
        {
            Console.WriteLine($"Sending notification to user {userId}: {message}");
            if (!string.IsNullOrEmpty(userId))
            {
                await Clients.User(userId).SendAsync("ReceiveNotification", message);
            }
        }
    }

}
