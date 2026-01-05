using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Submission
{
    public Guid Id { get; set; }

    public Guid? AssignmentId { get; set; }

    public Guid? StudentId { get; set; }

    public decimal? Score { get; set; }

    public string? TeacherFeedback { get; set; }

    public string? ResultSnapshot { get; set; }

    public DateTime? SubmittedAt { get; set; }

    public virtual Assignment? Assignment { get; set; }

    public virtual User? Student { get; set; }
}
