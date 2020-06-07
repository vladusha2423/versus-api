using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Versus.Data.Entities;

namespace Versus.Core.EF
{
    public class VersusContext: IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public DbSet<Exercise> Exercise { get; set; }
        public DbSet<Notifications> Notifications { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<Exercises> Exercises { get; set; }
        public DbSet<VIP> Vip { get; set; }

        public VersusContext(DbContextOptions<VersusContext> opt) : base(opt)
        {
            
            //Database.EnsureCreated();
        }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<IdentityRole<Guid>>().HasData(
                new IdentityRole<Guid>[]
                {
                    new IdentityRole<Guid>
                    {
                        Id = Guid.NewGuid(),
                        Name = "admin",
                        NormalizedName = "ADMIN"
                    },
                    new IdentityRole<Guid>
                    {
                        Id = Guid.NewGuid(),
                        Name = "user",
                        NormalizedName = "USER"
                    }
                }    
            );
            

            base.OnModelCreating(builder);
        }

    }
}