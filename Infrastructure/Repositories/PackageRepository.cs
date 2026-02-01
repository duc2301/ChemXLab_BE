using Application.Interfaces.IRepositories;
using Domain.Entities;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class PackageRepository : GenericRepository<Package>, IPackageRepository
    {
        private readonly ChemXlabContext _context;
        public PackageRepository(ChemXlabContext context) : base(context)
        {
            _context = context;
        }
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Packages.AnyAsync(p => p.Id == id);
        }

    }
}
