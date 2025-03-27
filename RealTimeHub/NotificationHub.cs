using Microsoft.AspNetCore.SignalR;

namespace RealTimeHub
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            Console.WriteLine($"User connected: {userId}");
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
                Console.WriteLine($"User {userId} added to group");
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            Console.WriteLine($"User disconnected: {userId}");
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
                Console.WriteLine($"User {userId} removed from group");
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendNotification(string message)
        {
            var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            Console.WriteLine($"Sending notification to user {userId}: {message}");
            if (!string.IsNullOrEmpty(userId))
            {
                await Clients.Group(userId).SendAsync("ReceiveNotification", message);
            }
        }
    }

}
