﻿using System;

namespace Versus.Data.Entities
{
    public class Settings
    {
        public Guid Id { get; set; }
        public User UserId { get; set; }
        public bool Language { get; set; }
        public bool Sound { get; set; }
        public bool Invites { get; set; }
        public Guid NotificationsId { get; set; } 
        public Notifications Notifications { get; set; } 
    }
}