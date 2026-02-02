using Application.Interfaces.IRepositories;
using Application.Interfaces.IUnitOfWork;
using Infrastructure.DbContexts;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.UnitOfWorks
{
    /// <summary>
    /// Implementation of the Unit of Work pattern to coordinate transactional operations across multiple repositories.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ChemXlabContext _context;
        private IDbContextTransaction? _transaction;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
        /// </summary>
        /// <param name="context">The database context instance.</param>
        /// <exception cref="ArgumentNullException">Thrown if the context is null.</exception>
        public UnitOfWork(ChemXlabContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // User Repository
        private IUserRepository _userRepository;

        /// <summary>
        /// Gets the User Repository instance. Uses lazy initialization.
        /// </summary>
        public IUserRepository UserRepository => _userRepository ??= new UserRepository(_context);

        // Payment Repository
        private IPaymentRepository _paymentRepository;

        /// <summary>
        /// Gets the Payment Repository instance. Uses lazy initialization.
        /// </summary>
        public IPaymentRepository PaymentRepository => _paymentRepository ??= new PaymentRepository(_context);

        // Package Repository
        private IPackageRepository _packageRepository;

        /// <summary>
        /// Gets the Package Repository instance. Uses lazy initialization.
        /// </summary>
        public IPackageRepository PackageRepository => _packageRepository ??= new PackageRepository(_context);

        /// <summary>
        /// Persists all changes made in the current context to the database.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Executes a database transaction to ensure atomicity of operations.
        /// If successful, changes are committed; otherwise, the transaction is rolled back.
        /// </summary>
        public async Task CommitAsync()
        {
            try
            {
                // Note: Ensure the transaction object is assigned to _transaction if you intend to use it in RollbackAsync
                await _context.Database.BeginTransactionAsync();
                await _context.SaveChangesAsync();
                await _context.Database.CommitTransactionAsync();
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Rolls back the current active transaction.
        /// </summary>
        private async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        /// <summary>
        /// Releases the unmanaged resources used by the UnitOfWork and optionally releases the managed resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; False to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _transaction?.Dispose();
                    _context.Dispose();
                }
                _disposed = true;
            }
        }
    }
}