using BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface INotificationService
    {
        Task NotifyUserAsync(int userId, string message, string articleId);
        Task<List<Notification>> GetUserNotificationsAsync(int userId);
        Task<List<Notification>> GetRecentNotificationsAsync(int count);
    }
}
