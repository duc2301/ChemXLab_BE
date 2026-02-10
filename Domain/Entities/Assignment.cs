using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Assignment
{
    public Guid Id { get; set; }

    public Guid? ClassId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? Deadline { get; set; }

    public string? Status { get; set; }

    public string? LabConfig { get; set; }

    public virtual Class? Class { get; set; }

    public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();
}
