using System;
using System.Collections.Generic;

namespace AutoShopWeb.Models;

public partial class Service
{
    public int ServiceId { get; set; }

    public string ServiceType { get; set; } = null!;

    public decimal Cost { get; set; }

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();
}
