using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.ResponseDTOs.Payment
{
    public class PaymentResponseDTO
    {
        public Guid Id { get; set; }

        public Guid? UserId { get; set; }

        public int? PackageId { get; set; }

        public decimal Amount { get; set; }

        public string? Currency { get; set; }

        public string? PaymentMethod { get; set; }

        public string? Status { get; set; }

        public string? QrUrl { get; set; }

        public string? TransactionCode { get; set; }

    }
}
