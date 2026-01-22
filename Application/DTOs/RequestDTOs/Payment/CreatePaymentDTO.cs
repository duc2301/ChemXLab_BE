using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.RequestDTOs.Payment
{
    public class CreatePaymentDTO
    {
        public Guid? UserId { get; set; }

        public int? PackageId { get; set; }

        public decimal Amount { get; set; }

        public string? Currency { get; set; }

        public string? PaymentMethod { get; set; }
    }
}
