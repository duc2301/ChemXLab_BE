using Application.Interfaces.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.IUnitOfWork
{
    /// <summary>
    /// Defines the Unit of Work pattern to coordinate data changes across multiple repositories.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Gets the repository for managing user entities.
        /// </summary>
        IUserRepository UserRepository { get; }

        /// <summary>
        /// Gets the repository for managing payment transactions.
        /// </summary>
        IPaymentRepository PaymentRepository { get; }

        /// <summary>
        /// Gets the repository for managing subscription packages.
        /// </summary>
        IPackageRepository PackageRepository { get; }

        ISubscriptionRepository SubscriptionRepository { get; }

        /// <summary>
        /// Saves all changes made in this context to the database asynchronously.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Commits the current transaction asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task CommitAsync();
    }
}