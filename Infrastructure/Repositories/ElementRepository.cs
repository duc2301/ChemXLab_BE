using Application.Interfaces.IRepositories;
using Domain.Entities;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for Element entity.
    /// </summary>
    public class ElementRepository : GenericRepository<Element>, IElementRepository
    {
        private readonly ChemXlabContext _context;

        public ElementRepository(ChemXlabContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Checks if an element with the given symbol already exists.
        /// </summary>
        public async Task<bool> ElementSymbolExistsAsync(string symbol)
        {
            return await _context.Elements.AnyAsync(e => e.Symbol.ToLower() == symbol.ToLower());
        }

        /// <summary>
        /// Retrieves an element by its chemical symbol.
        /// </summary>
        public async Task<Element?> GetBySymbolAsync(string symbol)
        {
            return await _context.Elements.FirstOrDefaultAsync(e => e.Symbol.ToLower() == symbol.ToLower());
        }
    }
}
