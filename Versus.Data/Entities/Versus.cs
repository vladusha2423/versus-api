using System;
using System.Collections.Generic;

namespace Versus.Data.Entities
{
    public class Versus
    {
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        public List<VersusUser> RejectedUser { get; set; }
        public Guid InitiatorId { get; set; }
        public string InitiatorName { get; set; }
        public Guid OpponentId { get; set; }
        public string OpponentName { get; set; }
        public string Exercise { get; set; }
        public string Status { get; set; }
        public Guid LastInvitedId { get; set; }
        public Guid FirstCompletedId { get; set; }
        public int InitiatorIterations { get; set; }
        public int OpponentIterations { get; set; }
        public string WinnerName { get; set; } 
    }
}