using System;
using System.Collections.Generic;

namespace Domain.Entities;

/// <summary>
/// Represents the many-to-many relationship between Classes and Students.
/// </summary>
public partial class ClassMember
{
    public Guid ClassId { get; set; }

    public Guid StudentId { get; set; }

    /// <summary>
    /// The date when the student joined the class.
    /// </summary>
    public DateTime? JoinedAt { get; set; }

    public virtual Class Class { get; set; } = null!;

    public virtual User Student { get; set; } = null!;
}