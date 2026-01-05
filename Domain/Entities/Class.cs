using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Class
{
    public Guid Id { get; set; }

    public Guid? TeacherId { get; set; }

    public string Name { get; set; } = null!;

    public string? ClassCode { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    public virtual ICollection<ClassMember> ClassMembers { get; set; } = new List<ClassMember>();

    public virtual User? Teacher { get; set; }
}
