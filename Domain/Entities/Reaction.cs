using System;
using System.Collections.Generic;

namespace Domain.Entities;

/// <summary>
/// Represents a chemical reaction defined in the system.
/// </summary>
public partial class Reaction
{
    public Guid Id { get; set; }

    /// <summary>
    /// A textual description of the reaction.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Required conditions for the reaction (e.g., temperature, pressure, catalyst).
    /// </summary>
    public string? Conditions { get; set; }

    /// <summary>
    /// Indicates if the reaction is reversible (equilibrium).
    /// </summary>
    public bool? IsReversible { get; set; }

    /// <summary>
    /// URL to an educational video demonstrating the reaction.
    /// </summary>
    public string? VideoUrl { get; set; }

    /// <summary>
    /// Configuration for visualizing the reaction in the lab interface.
    /// </summary>
    public string? VisualConfig { get; set; }

    public virtual ICollection<ReactionComponent> ReactionComponents { get; set; } = new List<ReactionComponent>();
}