using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.IServices
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
