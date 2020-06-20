using System.Threading.Tasks;

namespace Versus.Messaging.Interfaces
{
    public interface IMobileMessagingClient
    {
        Task<object> SendNotification(string token, string title, string body);
        Task<object> SendData(string token, string title, string body);
    }
}