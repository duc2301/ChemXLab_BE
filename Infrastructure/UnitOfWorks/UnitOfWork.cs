using Application.Interfaces.IRepositories;
using Application.Interfaces.IUnitOfWork;
using Infrastructure.DbContexts;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ChemXlabContext _context;
        private IDbContextTransaction? _transaction;
        private bool _disposed;

        public UnitOfWork(ChemXlabContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // User Repository
        private IUserRepository _userRepository;
        public IUserRepository UserRepository => _userRepository ??= new UserRepository(_context);

        // Payment Repository
        private IPaymentRepository _paymentRepository;
        public IPaymentRepository PaymentRepository => _paymentRepository ??= new PaymentRepository(_context);

        // Package Repository
        private IPackageRepository _packageRepository;
        public IPackageRepository PackageRepository => _packageRepository ??= new PackageRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task CommitAsync()
        {
            try
            {
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

        private async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

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
