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
    }
}