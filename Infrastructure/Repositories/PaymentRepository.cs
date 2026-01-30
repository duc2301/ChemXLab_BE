using Application.Interfaces.IRepositories;
using Domain.Entities;
using Infrastructure.DbContexts;

namespace Infrastructure.Repositories
{
    public class PaymentRepository : GenericRepository<PaymentTransaction>, IPaymentRepository
    {
        public PaymentRepository(ChemXlabContext context) : base(context)
        {
        }
    }
}
