using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Versus.Data.Entities
{
    public class User : IdentityUser<Guid>
    {
        [Required]
        public string Token { get; set; }
        public string Country { get; set; }
        public Settings Settings { get; set; }
        public VIP Vip { get; set; }
        public Exercises Exercises { get; set; }
    }
}