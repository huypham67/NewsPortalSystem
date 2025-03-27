using BusinessObjects.Entities;
using Microsoft.AspNetCore.SignalR;
using RealTimeHub;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepo;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(INotificationRepository notificationRepo, IHubContext<NotificationHub> hubContext)
        {
            _notificationRepo = notificationRepo;
            _hubContext = hubContext;
        }

        public async Task NotifyUserAsync(int userId, string message, string articleId)
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                ArticleId = articleId
            };
            // Debug log kiểm tra có gửi đúng userId không
            Console.WriteLine($"[NotifyUserAsync] Sending notification to user {userId}: {message}");
            // Gửi thông báo qua SignalR (dùng Groups thay vì Clients.User)
            await _hubContext.Clients.Group(userId.ToString()).SendAsync("ReceiveNotification", message);
        }

        public async Task<List<Notification>> GetUserNotificationsAsync(int userId)
        {
            return await _notificationRepo.GetUserNotificationsAsync(userId);
        }
        public async Task<List<Notification>> GetRecentNotificationsAsync(int count)
        {
            return await _notificationRepo.GetRecentNotificationsAsync(count);
        }
    }
}
