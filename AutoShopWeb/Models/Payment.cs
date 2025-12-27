using System;
using System.Collections.Generic;

namespace AutoShopWeb.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int ContractNumber { get; set; }

    public decimal Amount { get; set; }

    public string Status { get; set; } = null!;

    public DateOnly PayDate { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public virtual Contract ContractNumberNavigation { get; set; } = null!;
}
