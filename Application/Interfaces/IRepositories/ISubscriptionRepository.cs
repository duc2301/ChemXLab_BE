using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.IRepositories
{
    public interface ISubscriptionRepository : IGenericRepository<Subscription>
    {
        Task<IEnumerable<Subscription>> getMySubscription(Guid value);
    }
}
