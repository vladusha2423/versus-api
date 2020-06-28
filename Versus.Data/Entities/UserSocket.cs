using System;

namespace Versus.Data.Entities
{
    public class UserSocket
    {
        public Guid UserId { get; set; }
        public string SocketId { get; set; }
        public DateTime LastTime { get; set; }
    }
}