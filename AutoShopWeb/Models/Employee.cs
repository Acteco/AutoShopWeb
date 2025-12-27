using System;
using System.Collections.Generic;

namespace AutoShopWeb.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public string Position { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string? Email { get; set; }

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
