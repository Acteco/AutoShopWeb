using System;
using System.Collections.Generic;

namespace AutoShopWeb.Models;

public partial class Contract
{
    public int ContractNumber { get; set; }

    public int ClientId { get; set; }

    public int CarId { get; set; }

    public int EmployeeId { get; set; }

    public int? ServiceId { get; set; }

    public decimal TotalAmount { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public DateOnly CreatedAt { get; set; }

    public virtual Car Car { get; set; } = null!;

    public virtual Client Client { get; set; } = null!;

    public virtual Employee Employee { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Service? Service { get; set; }
}
