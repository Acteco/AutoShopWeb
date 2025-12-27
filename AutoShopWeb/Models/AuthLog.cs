using System;
using System.Collections.Generic;

namespace AutoShopWeb.Models;

public partial class AuthLog
{
    public long LogId { get; set; }

    public int? UserId { get; set; }

    public string Login { get; set; } = null!;

    public bool IsSuccess { get; set; }

    public string? Reason { get; set; }

    public DateTime LoggedAt { get; set; }

    public virtual User? User { get; set; }
}
