using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Chemical
{
    public Guid Id { get; set; }

    public string Formula { get; set; } = null!;

    public string? CommonName { get; set; }

    public string? IupacName { get; set; }

    public string? StateAtRoomTemp { get; set; }

    public string? Structure3dUrl { get; set; }

    public string? MolecularData { get; set; }

    public bool? IsPublic { get; set; }

    public Guid? CreatedBy { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<ReactionComponent> ReactionComponents { get; set; } = new List<ReactionComponent>();
}
