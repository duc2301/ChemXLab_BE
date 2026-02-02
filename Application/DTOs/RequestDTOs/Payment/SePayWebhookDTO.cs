using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.RequestDTOs.Payment
{
    /// <summary>
    /// Data Transfer Object for handling Webhook data received from SePay.
    /// </summary>
    public class SePayWebhookDTO
    {
        /// <summary>
        /// The unique transaction identifier from the payment gateway.
        /// </summary>
        public Guid TransactionId { get; set; }

        /// <summary>
        /// The content or description of the transaction (e.g., transfer memo).
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// The amount of money transferred.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// The status of the transaction (e.g., "Success", "Failed").
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// The date and time when the transaction occurred.
        /// </summary>
        public DateTime TransactionDate { get; set; }
        // Add other fields according to SePay IPN documentation
    }
}