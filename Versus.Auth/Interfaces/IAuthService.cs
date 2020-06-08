using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Versus.Data.Dto;

namespace Versus.Auth.Interfaces
{
    public interface IAuthService
    {
        Task<object> Login(string username, string password);

        Task<object> Register(UserDto item);
    }
    

}
