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
        /// The identifier of the subscription package being purchased.
        /// </summary>
        public Guid? PackageId { get; set; }
    }
}