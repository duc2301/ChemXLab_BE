using Application.Interfaces.IRepositories;
using Domain.Entities;
using Infrastructure.DbContexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class PaymentRepository : GenericRepository<PaymentTransaction>, IPaymentRepository
    {
        public PaymentRepository(ChemXlabContext context) : base(context)
        {
        }
    }
}
