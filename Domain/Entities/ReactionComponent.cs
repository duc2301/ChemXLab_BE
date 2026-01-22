using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class ReactionComponent
{
    public Guid Id { get; set; }

    public Guid? ReactionId { get; set; }

    public Guid? ChemicalId { get; set; }

    public string Role { get; set; } = null!;

    public int? Coefficient { get; set; }

    public string? StateInReaction { get; set; }

    public virtual Chemical? Chemical { get; set; }

    public virtual Reaction? Reaction { get; set; }
}
