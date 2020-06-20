using Microsoft.AspNetCore.SignalR;

namespace Versus.Configurations
{
    public class UserIdProvider : IUserIdProvider
    {
        public virtual string GetUserId(HubConnectionContext connection)
                {
                    return connection.User?.Identity.Name;
                    // или так
                    //return connection.User?.FindFirst(ClaimTypes.Name)?.Value;
                }
    }
}