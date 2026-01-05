using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Element
{
    public int Id { get; set; }

    public string Symbol { get; set; } = null!;

    public string Name { get; set; } = null!;

    public decimal? AtomicMass { get; set; }

    public string? Properties { get; set; }
}
