using System;
using System.Collections.Generic;

namespace Domain.Entities;

/// <summary>
/// Represents a chemical substance defined in the system.
/// </summary>
public partial class Chemical
{
    public Guid Id { get; set; }

    /// <summary>
    /// The chemical formula (e.g., H2O, CO2).
    /// </summary>
    public string Formula { get; set; } = null!;

    /// <summary>
    /// The common name of the chemical.
    /// </summary>
    public string? CommonName { get; set; }

    /// <summary>
    /// The standard IUPAC name.
    /// </summary>
    public string? IupacName { get; set; }

    /// <summary>
    /// The physical state of the chemical at room temperature (Solid, Liquid, Gas).
    /// </summary>
    public string? StateAtRoomTemp { get; set; }

    /// <summary>
    /// URL to the 3D model resource for visualization.
    /// </summary>
    public string? Structure3dUrl { get; set; }

    /// <summary>
    /// Additional molecular data stored as JSON.
    /// </summary>
    public string? MolecularData { get; set; }

    /// <summary>
    /// Indicates if this chemical is visible to all users or private to the creator.
    /// </summary>
    public bool? IsPublic { get; set; }

    /// <summary>
    /// The ID of the user who created this custom chemical (if applicable).
    /// </summary>
    public Guid? CreatedBy { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<ReactionComponent> ReactionComponents { get; set; } = new List<ReactionComponent>();
}