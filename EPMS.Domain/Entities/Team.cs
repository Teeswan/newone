using System;
using System.Collections.Generic;

using EPMS.Domain.Interfaces;

namespace EPMS.Domain.Entities;

public partial class Team : ISoftDelete
{
    public int TeamId { get; set; }

    public string TeamName { get; set; } = null!;

    public bool? IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public int? ManagerId { get; set; }

    public int? DepartmentId { get; set; }

    public virtual Department? Department { get; set; }

    public virtual Employee? Manager { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
