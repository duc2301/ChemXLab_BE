using Application.Interfaces.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.IUnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }
        IPaymentRepository PaymentRepository { get; }
        Task<int> SaveChangesAsync();
        Task CommitAsync();
    }
}
