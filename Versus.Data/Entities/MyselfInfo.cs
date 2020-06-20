using System;

namespace Versus.Data.Entities
{
    public class MyselfInfo
    {
        public Guid UserId { get; set; }
        public object Jwt { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
    }
}