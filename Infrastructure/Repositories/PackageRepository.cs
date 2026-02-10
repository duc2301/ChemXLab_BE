using Application.Interfaces.IRepositories;
using Domain.Entities;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for managing Subscription Package entities.
    /// </summary>
    public class PackageRepository : GenericRepository<Package>, IPackageRepository
    {
        private readonly ChemXlabContext _context;

        public PackageRepository(ChemXlabContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Checks if a package with the specified ID exists in the database.
        /// </summary>
        /// <returns>True if the package exists, otherwise False.</returns>
        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Packages.AnyAsync(p => p.Id == id);
        }
    }
}