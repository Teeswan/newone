using System;
using System.Collections.Generic;

namespace EPMS.Domain.Entities;

public partial class Team
{
    public int TeamId { get; set; }

    public string TeamName { get; set; } = null!;

    public int? ManagerId { get; set; }

    public int? DepartmentId { get; set; }

    public virtual Department? Department { get; set; }

    public virtual Employee? Manager { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
