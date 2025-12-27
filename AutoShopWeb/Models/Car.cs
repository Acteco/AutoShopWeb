using System;
using System.Collections.Generic;

namespace AutoShopWeb.Models;

public partial class Car
{
    public int CarId { get; set; }

    public string Model { get; set; } = null!;

    public int Mileage { get; set; }

    public int ReleaseYear { get; set; }

    public decimal Price { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();
}
