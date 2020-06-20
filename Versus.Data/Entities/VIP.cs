using System;

namespace Versus.Data.Entities
{
    // ReSharper disable once InconsistentNaming
    public class VIP
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public DateTime Begin { get; set; }
        public int Duration { get; set; }
    }
}