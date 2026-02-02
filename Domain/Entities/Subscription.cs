using System;
using System.Collections.Generic;

namespace Domain.Entities;

/// <summary>
/// Represents a user's active or past subscription to a package.
/// </summary>
public partial class Subscription
{
    public Guid Id { get; set; }

    public Guid? UserId { get; set; }

    public int? PackageId { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Indicates whether the subscription is currently valid.
    /// </summary>
    public bool? IsActive { get; set; }

    public virtual Package? Package { get; set; }

    public virtual User? User { get; set; }
}