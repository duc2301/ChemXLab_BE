using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.IRepositories
{
    /// <summary>
    /// Repository interface for Element entity CRUD operations.
    /// </summary>
    public interface IElementRepository : IGenericRepository<Domain.Entities.Element>
    {
        /// <summary>
        /// Checks if an element with the given symbol already exists.
        /// </summary>
        Task<bool> ElementSymbolExistsAsync(string symbol);

        /// <summary>
        /// Retrieves an element by its chemical symbol.
        /// </summary>
        Task<Domain.Entities.Element?> GetBySymbolAsync(string symbol);
    }
}
