using System;
using Microsoft.AspNetCore.Identity;

namespace Versus.Models
{
    public class User: IdentityUser<Guid>
    {
        public string Name { get; set; }
        public string Country { get; set; }
    }
}