using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Subscription
{
    public Guid Id { get; set; }

    public Guid? UserId { get; set; }

    public int? PackageId { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual Package? Package { get; set; }

    public virtual User? User { get; set; }
}
