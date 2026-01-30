using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.RequestDTOs.Payment
{
    public class SePayWebhookDTO
    {
        public Guid TransactionId { get; set; }

        public string Content { get; set; }

        public decimal Amount { get; set; }

        public string Status { get; set; }

        public DateTime TransactionDate { get; set; }
        // Thêm các field khác theo IPN của SePay
    }
}
