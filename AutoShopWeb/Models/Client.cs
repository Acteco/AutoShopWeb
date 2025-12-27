using System;
using System.Collections.Generic;

namespace AutoShopWeb.Models;

public partial class Client
{
    public int ClientId { get; set; }

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public string PassportSeries { get; set; } = null!;

    public string PassportNumber { get; set; } = null!;

    public DateOnly BirthDate { get; set; }

    public DateOnly PassportIssueDate { get; set; }

    public string Phone { get; set; } = null!;

    public string? Email { get; set; }

    public string? Address { get; set; }

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
