using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.ResponseDTOs.Subscriptions
{
    public class SubscriptionResponseDTO
    {
        public Guid Id { get; set; }

        public Guid? UserId { get; set; }

        public int? PackageId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool? IsActive { get; set; }
    }
}
