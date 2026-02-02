using System;
using System.Collections.Generic;

namespace Domain.Entities;

/// <summary>
/// Represents a chemical element from the periodic table.
/// </summary>
public partial class Element
{
    /// <summary>
    /// The atomic number of the element.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The chemical symbol (e.g., H, He, Li).
    /// </summary>
    public string Symbol { get; set; } = null!;

    /// <summary>
    /// The full name of the element.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// The atomic mass of the element.
    /// </summary>
    public decimal? AtomicMass { get; set; }

    /// <summary>
    /// Additional properties (e.g., group, period, electron configuration) stored as JSON.
    /// </summary>
    public string? Properties { get; set; }
}