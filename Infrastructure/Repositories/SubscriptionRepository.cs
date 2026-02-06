using Application.Interfaces.IRepositories;
using Domain.Entities;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class SubscriptionRepository : GenericRepository<Subscription>, ISubscriptionRepository
    {
        private readonly ChemXlabContext _context;
        public SubscriptionRepository(ChemXlabContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Subscription>> getMySubscription(Guid value)
        {
            return await _context.Subscriptions
                .Include(s => s.Package)
                .Where(s => s.UserId == value && s.IsActive == true).ToListAsync() ;
        }

    }
}
