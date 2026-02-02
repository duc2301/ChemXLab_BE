using System;
using System.Collections.Generic;

namespace Domain.Entities;

/// <summary>
/// Represents a virtual classroom managed by a teacher.
/// </summary>
public partial class Class
{
    public Guid Id { get; set; }

    /// <summary>
    /// The ID of the teacher who owns the class.
    /// </summary>
    public Guid? TeacherId { get; set; }

    /// <summary>
    /// The display name of the class.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// A unique code used by students to join the class.
    /// </summary>
    public string? ClassCode { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    public virtual ICollection<ClassMember> ClassMembers { get; set; } = new List<ClassMember>();

    public virtual User? Teacher { get; set; }
}