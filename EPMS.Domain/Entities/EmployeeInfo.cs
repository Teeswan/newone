using System;
using System.Collections.Generic;

namespace EPMS.Domain.Entities;

public partial class EmployeeInfo
{
    public int InfoId { get; set; }

    public int? EmployeeId { get; set; }

    public string? Gender { get; set; }

    public string? Phone { get; set; }

    public string? EmergencyContact { get; set; }

    public string? EmergencyPhone { get; set; }

    public string? BankName { get; set; }

    public string? BankAccountNumber { get; set; }

    public string? NrcNumber { get; set; }

    public DateTime? LastUpdated { get; set; }

    public virtual Employee? Employee { get; set; }
}
