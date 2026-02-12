using Application.Interfaces.IRepositories;
using Domain.Entities;
using Infrastructure.DbContexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class ChemicalRepository : GenericRepository<Chemical>, IChemicalRepository
    {
        public ChemicalRepository(ChemXlabContext context) : base(context)
        {
        }
    }
}
