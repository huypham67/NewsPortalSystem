using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Entities
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        public int UserId { get; set; }  // Người nhận thông báo
        public string? ArticleId { get; set; } // Bài viết liên quan
        public string Message { get; set; } = null!;
        public bool IsRead { get; set; } = false;  // Đánh dấu đã đọc hay chưa

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("ArticleId")]
        public virtual Article? Article { get; set; }
    }

}
