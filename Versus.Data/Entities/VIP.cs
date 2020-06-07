using System;
using System.Dynamic;

namespace Versus.Data.Entities
{
    public class VIP
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public DateTime Begin { get; set; }
        public int Duration { get; set; }
    }
}