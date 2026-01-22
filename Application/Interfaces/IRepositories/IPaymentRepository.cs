using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.IRepositories
{
    public interface IPaymentRepository : IGenericRepository<PaymentTransaction>
    {
    }
}
