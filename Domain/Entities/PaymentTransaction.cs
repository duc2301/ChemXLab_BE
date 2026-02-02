using System;
using System.Collections.Generic;

namespace Domain.Entities;

/// <summary>
/// Records a payment attempt or completed transaction for a package.
/// </summary>
public partial class PaymentTransaction
{
    public Guid Id { get; set; }

    public Guid? UserId { get; set; }

    public int? PackageId { get; set; }

    public decimal Amount { get; set; }

    public string? Currency { get; set; }

    /// <summary>
    /// The method used for payment (e.g., SEPAY, MOMO, VISA).
    /// </summary>
    public string? PaymentMethod { get; set; }

    /// <summary>
    /// The generated URL for the payment QR code.
    /// </summary>
    public string? Qrurl { get; set; }

    /// <summary>
    /// Description or memo for the transaction.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The date and time when the payment was confirmed.
    /// </summary>
    public DateTime? PaidAt { get; set; }

    /// <summary>
    /// The current status of the transaction (PENDING, SUCCESS, FAILED, CANCELLED).
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// A unique code generated for this transaction reference.
    /// </summary>
    public string? TransactionCode { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Package? Package { get; set; }

    public virtual User? User { get; set; }
}