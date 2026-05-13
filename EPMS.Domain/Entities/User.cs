using System;
using System.Collections.Generic;

namespace EPMS.Domain.Entities;

public partial class User
{
    public int UserId { get; set; }

    public int? EmployeeId { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
