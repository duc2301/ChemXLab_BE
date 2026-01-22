using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class PaymentTransaction
{
    public Guid Id { get; set; }

    public Guid? UserId { get; set; }

    public int? PackageId { get; set; }

    public decimal Amount { get; set; }

    public string? Currency { get; set; }

    public string? PaymentMethod { get; set; }

    public string? Status { get; set; }

    public string? TransactionCode { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Package? Package { get; set; }

    public virtual User? User { get; set; }
}
