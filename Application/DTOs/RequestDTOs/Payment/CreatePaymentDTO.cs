using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.RequestDTOs.Payment
{
    /// <summary>
    /// Data Transfer Object for initiating a new payment request.
    /// </summary>
    public class CreatePaymentDTO
    {
        /// <summary>
        /// The unique identifier of the user making the payment.
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// The identifier of the subscription package being purchased.
        /// </summary>
        public int? PackageId { get; set; }

        /// <summary>
        /// The total amount to be paid.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// The currency code (e.g., "VND", "USD").
        /// </summary>
        public string? Currency { get; set; }

        /// <summary>
        /// The selected method of payment (e.g., "BankTransfer", "CreditCard").
        /// </summary>
        public string? PaymentMethod { get; set; }
    }
}