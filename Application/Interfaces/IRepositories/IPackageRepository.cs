using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.IRepositories
{
    /// <summary>
    /// Repository interface for managing Subscription Packages.
    /// </summary>
    public interface IPackageRepository : IGenericRepository<Package>
    {
        /// <summary>
        /// Checks if a package exists by its ID.
        /// </summary>
        /// <returns>True if the package exists, otherwise False.</returns>
        Task<bool> ExistsAsync(int id);
    }
}