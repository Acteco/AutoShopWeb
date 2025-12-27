using System;
using System.Collections.Generic;

namespace AutoShopWeb.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Login { get; set; } = null!;

    public byte[] PasswordHash { get; set; } = null!;

    public byte[] PasswordSalt { get; set; } = null!;

    public int RoleId { get; set; }

    public int? ClientId { get; set; }

    public int? EmployeeId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public virtual ICollection<AuthLog> AuthLogs { get; set; } = new List<AuthLog>();

    public virtual Client? Client { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual Role Role { get; set; } = null!;
}
