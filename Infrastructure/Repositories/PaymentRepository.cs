using Application.Interfaces.IRepositories;
using Domain.Entities;
using Infrastructure.DbContexts;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for managing Payment Transaction entities.
    /// </summary>
    public class PaymentRepository : GenericRepository<PaymentTransaction>, IPaymentRepository
    {
        public PaymentRepository(ChemXlabContext context) : base(context)
        {
        }
    }
}