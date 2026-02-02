using System;
using System.Collections.Generic;

namespace Domain.Entities;

/// <summary>
/// Represents a task, homework, or lab experiment assigned to a specific class.
/// </summary>
public partial class Assignment
{
    /// <summary>
    /// Unique identifier for the assignment.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The ID of the class this assignment belongs to.
    /// </summary>
    public Guid? ClassId { get; set; }

    /// <summary>
    /// The title of the assignment.
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// Detailed instructions or description of the task.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The date and time by which the assignment must be submitted.
    /// </summary>
    public DateTime? Deadline { get; set; }

    /// <summary>
    /// Configuration data for the virtual lab environment (e.g., allowed chemicals, tools).
    /// </summary>
    public string? LabConfig { get; set; }

    public virtual Class? Class { get; set; }

    public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();
}