using Application.Interfaces.IRepositories;
using Domain.Entities;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for managing Payment Transaction entities.
    /// </summary>
    public class PaymentRepository : GenericRepository<PaymentTransaction>, IPaymentRepository
    {
        private readonly ChemXlabContext _context;
        public PaymentRepository(ChemXlabContext context) : base(context)
        {
            _context = context;
        }

        public async Task ExpirePendingPaymentsAsync()
        {
            var listExspirePayment = await _context.PaymentTransactions
                .Where(p => p.Status == "PENDING" && p.CreatedAt <= DateTime.Now.AddMinutes(-5))
                .ToListAsync();

            foreach (var payment in listExspirePayment)
            {
                payment.Status = "EXPIRED";
            }
        }

        public async Task<PaymentTransaction?> GetByTransactionCodeAsync(string code)
        {
            return await _context.PaymentTransactions.FirstOrDefaultAsync(p =>  p.TransactionCode == code);
        }

        public async Task<IEnumerable<PaymentTransaction>> GetByUserId(Guid userId)
        {
            return await _context.PaymentTransactions.Where(p => p.UserId == userId).ToListAsync();
        }

        public async Task<IEnumerable<PaymentTransaction>> GetPendingPaymentsByAmountAsync(decimal amount)
        {
            return await _context.PaymentTransactions.Where(p => p.Status == "PENDING" && p.Amount == amount).ToListAsync();
        }
    }
}