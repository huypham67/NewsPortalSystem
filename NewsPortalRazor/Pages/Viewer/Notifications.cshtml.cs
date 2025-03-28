using BusinessObjects.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace NewsPortalRazor.Pages.Viewer
{
    [Authorize(Roles = "Viewer")]
    public class NotificationsModel : PageModel
    {
        private readonly INotificationService _notificationService;

        public NotificationsModel(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public List<Notification> Notifications { get; set; }

        public async Task OnGetAsync()
        {
            Notifications = await _notificationService.GetRecentNotificationsAsync(10);
        }
    }
}
