using System.Threading.Tasks;
using Versus.Data.Dto;
using Versus.Data.Entities;

namespace Versus.Auth.Interfaces
{
    public interface IAuthService
    {
        Task<Myself> Login(string username, string password);

        Task<object> Logout();

        Task<Myself> Register(UserDto item);
    }
    

}
