using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.ResponseDTOs.Payment
{
    /// <summary>
    /// Data Transfer Object representing the details of a payment transaction.
    /// </summary>
    public class PaymentResponseDTO
    {
        /// <summary>
        /// The unique identifier of the payment record.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The identifier of the user who made the payment.
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// The identifier of the purchased package.
        /// </summary>
        public int? PackageId { get; set; }

        /// <summary>
        /// The amount paid.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// The currency used for the payment.
        /// </summary>
        public string? Currency { get; set; }

        /// <summary>
        /// The method used for payment.
        /// </summary>
        public string? PaymentMethod { get; set; }

        /// <summary>
        /// The current status of the payment (e.g., "Pending", "Completed").
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// The URL to the QR code for payment (if applicable).
        /// </summary>
        public string? QrUrl { get; set; }

        /// <summary>
        /// The transaction code or reference number.
        /// </summary>
        public string? TransactionCode { get; set; }
    }
}