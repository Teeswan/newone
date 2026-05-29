using System;
using System.Collections.Generic;

using EPMS.Domain.Interfaces;

namespace EPMS.Domain.Entities;

public partial class Department : ISoftDelete
{
    public int DepartmentId { get; set; }

    public string? DepartmentIdName { get; set; }

    public string DepartmentName { get; set; } = null!;

    public bool? IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public int? ParentDepartmentId { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<Department> InverseParentDepartment { get; set; } = new List<Department>();

    public virtual Department? ParentDepartment { get; set; }

    public virtual ICollection<Team> Teams { get; set; } = new List<Team>();

    public virtual ICollection<DepartmentKpi> DepartmentKpis { get; set; } = new List<DepartmentKpi>();
}
