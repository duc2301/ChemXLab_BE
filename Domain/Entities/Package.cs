using System;
using System.Collections.Generic;

namespace Domain.Entities;

/// <summary>
/// Represents a subscription plan available for purchase.
/// </summary>
public partial class Package
{
    public int Id { get; set; }

    /// <summary>
    /// The display name of the package (e.g., "Premium Monthly").
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// The price of the package.
    /// </summary>
    public decimal? Price { get; set; }

    /// <summary>
    /// The duration of the subscription in days.
    /// </summary>
    public int? DurationDays { get; set; }

    /// <summary>
    /// A JSON string or delimited list describing the features included.
    /// </summary>
    public string? Features { get; set; }

    public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}