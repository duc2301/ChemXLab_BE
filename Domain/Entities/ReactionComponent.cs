using System;
using System.Collections.Generic;

namespace Domain.Entities;

/// <summary>
/// Represents a chemical participating in a reaction (either as a reactant or a product).
/// </summary>
public partial class ReactionComponent
{
    public Guid Id { get; set; }

    public Guid? ReactionId { get; set; }

    public Guid? ChemicalId { get; set; }

    /// <summary>
    /// The role of the component in the reaction ("Reactant" or "Product").
    /// </summary>
    public string Role { get; set; } = null!;

    /// <summary>
    /// The stoichiometric coefficient balancing the reaction.
    /// </summary>
    public int? Coefficient { get; set; }

    /// <summary>
    /// The physical state of the component during this specific reaction.
    /// </summary>
    public string? StateInReaction { get; set; }

    public virtual Chemical? Chemical { get; set; }

    public virtual Reaction? Reaction { get; set; }
}