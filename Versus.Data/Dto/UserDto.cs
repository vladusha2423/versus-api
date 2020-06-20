using System;
using Versus.Data.Entities;

namespace Versus.Data.Dto
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public bool Online { get; set; }

        public DateTime LastTime { get; set; }
        public string UserName { get; set; }
        public string Country { get; set; }
        public string Password { get; set; }
        public Settings Settings { get; set; }
        
        public bool IsVip { get; set; }
        public VIP Vip { get; set; }
        public Exercises Exercises { get; set; }
    }
}