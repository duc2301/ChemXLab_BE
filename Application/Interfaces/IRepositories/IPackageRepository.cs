using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.IRepositories
{
    public interface IPackageRepository : IGenericRepository<Package>
    {
        Task<bool> ExistsAsync(int id);
    }
}
