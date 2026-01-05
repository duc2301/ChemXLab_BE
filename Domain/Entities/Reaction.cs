using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Reaction
{
    public Guid Id { get; set; }

    public string? Description { get; set; }

    public string? Conditions { get; set; }

    public bool? IsReversible { get; set; }

    public string? VideoUrl { get; set; }

    public string? VisualConfig { get; set; }

    public virtual ICollection<ReactionComponent> ReactionComponents { get; set; } = new List<ReactionComponent>();
}
