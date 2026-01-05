using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Package
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public decimal? Price { get; set; }

    public int? DurationDays { get; set; }

    public string? Features { get; set; }

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
