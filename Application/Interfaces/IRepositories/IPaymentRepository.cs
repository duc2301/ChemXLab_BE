using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.IRepositories
{
    /// <summary>
    /// Repository interface for managing Payment Transactions.
    /// </summary>
    public interface IPaymentRepository : IGenericRepository<PaymentTransaction>
    {
        Task<PaymentTransaction?> GetByTransactionCodeAsync(string code);
        Task<IEnumerable<PaymentTransaction>> GetPendingPaymentsByAmountAsync(decimal amount);
    }
}