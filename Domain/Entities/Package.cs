using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Package
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal? Price { get; set; }

    public int? DurationDays { get; set; }

    public string? Status { get; set; }

    public string? Features { get; set; }

    public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
