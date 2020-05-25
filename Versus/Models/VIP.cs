using System;
using System.Dynamic;

namespace Versus.Models
{
    public class VIP
    {
        public Guid Id { get; set; }
        public User UserId { get; set; }
        public DateTime Begin { get; set; }
        public int Duration { get; set; }
    }
}