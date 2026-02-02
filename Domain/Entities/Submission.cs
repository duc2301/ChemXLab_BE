using System;
using System.Collections.Generic;

namespace Domain.Entities;

/// <summary>
/// Represents a student's submission for an assignment.
/// </summary>
public partial class Submission
{
    public Guid Id { get; set; }

    public Guid? AssignmentId { get; set; }

    public Guid? StudentId { get; set; }

    /// <summary>
    /// The score awarded by the teacher.
    /// </summary>
    public decimal? Score { get; set; }

    /// <summary>
    /// Feedback comments provided by the teacher.
    /// </summary>
    public string? TeacherFeedback { get; set; }

    /// <summary>
    /// A snapshot of the experiment result (e.g., JSON data, image URL).
    /// </summary>
    public string? ResultSnapshot { get; set; }

    public DateTime? SubmittedAt { get; set; }

    public virtual Assignment? Assignment { get; set; }

    public virtual User? Student { get; set; }
}