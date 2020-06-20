using System.Threading.Tasks;
using Versus.Data.Entities;

namespace Versus.Auth.Interfaces
{
    public interface IJwtGenerator
    {
        Task<object> GenerateJwt(User user);
    }

}
