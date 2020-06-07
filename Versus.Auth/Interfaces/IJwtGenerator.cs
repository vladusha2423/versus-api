using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Versus.Data.Entities;

namespace Versus.Auth.Interfaces
{
    public interface IJwtGenerator
    {
        Task<object> GenerateJwt(User user);
    }

}
