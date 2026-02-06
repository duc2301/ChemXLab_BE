using Application.DTOs.ResponseDTOs.Subscriptions;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.IServices
{
    public interface ISubscriptionService
    {
        Task<bool> ActiveSubscription(Guid? userId, int? packageId);

        Task<IEnumerable<SubscriptionResponseDTO>> MySubscription(Guid? userId);
    }
}
