using BusinessObjects.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using System.Security.Claims;

namespace NewsPortalRazor.Pages.Editor
{
    [Authorize(Roles = "Editor")]
    public class NotificationsModel : PageModel
    {
        private readonly INotificationService _notificationService;

        public NotificationsModel(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public List<Notification> Notifications { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                return RedirectToPage("/Login");
            }

            Notifications = await _notificationService.GetUserNotificationsAsync(userId);
            return Page();
        }
    }
}
