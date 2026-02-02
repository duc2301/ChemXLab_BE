using System;
using System.Collections.Generic;

namespace Domain.Entities;

/// <summary>
/// Represents a registered user in the ChemXLab system.
/// </summary>
public partial class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? FullName { get; set; }

    /// <summary>
    /// The role of the user (e.g., Student, Teacher, Admin).
    /// </summary>
    public string? Role { get; set; }

    public string? AvatarUrl { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Chemical> Chemicals { get; set; } = new List<Chemical>();

    public virtual ICollection<ClassMember> ClassMembers { get; set; } = new List<ClassMember>();

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();

    public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();

    public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}