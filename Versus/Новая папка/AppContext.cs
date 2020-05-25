using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Versus.Models;

namespace Versus.Context
{
    public class AppContext: IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public DbSet<Exercise> Exercise { get; set; }
        public DbSet<Notifications> Notifications { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<UserExercises> UserExercises { get; set; }
        public DbSet<VIP> VIP { get; set; }
        
        public AppContext(DbContextOptions<AppContext> opt): base(opt)
        {
            // Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            
        }
    }
}