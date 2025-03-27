using System;
using System.Collections.Generic;

namespace BusinessObjects.Entities;

public partial class User
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Password { get; set; } // Cho phép NULL với tài khoản Google

    public string Role { get; set; } = null!;

    public bool IsActive { get; set; }

    public bool IsExternalUser { get; set; } = false;

    public DateTime? CreatedAt { get; set; }

    public DateTime? LastLoginAt { get; set; }
}
