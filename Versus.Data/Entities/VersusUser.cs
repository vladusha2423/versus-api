using System;

namespace Versus.Data.Entities
{
    public class VersusUser
    {
        public Guid VersusId { get; set; }
        public Versus Versus { get; set; }
        public Guid UserId { get; set; }
    }
}