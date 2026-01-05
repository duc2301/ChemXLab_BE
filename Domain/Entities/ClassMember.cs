using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class ClassMember
{
    public Guid ClassId { get; set; }

    public Guid StudentId { get; set; }

    public DateTime? JoinedAt { get; set; }

    public virtual Class Class { get; set; } = null!;

    public virtual User Student { get; set; } = null!;
}
