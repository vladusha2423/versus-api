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
        public DbSet<Champion> Champions { get; set; }
        public DbSet<Data.Entities.Versus> Versus { get; set; }
        public DbSet<VersusUser> VersusUsers { get; set; }
        public DbSet<UserSocket> UserSockets { get; set; }

        public VersusContext(DbContextOptions<VersusContext> opt) : base(opt)
        {
            
            //Database.EnsureCreated();
        }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // builder.Entity<IdentityRole<Guid>>().HasData(
            //     new IdentityRole<Guid>[]
            //     {
            //         new IdentityRole<Guid>
            //         {
            //             Id = Guid.NewGuid(),
            //             Name = "admin",
            //             NormalizedName = "ADMIN"
            //         },
            //         new IdentityRole<Guid>
            //         {
            //             Id = Guid.NewGuid(),
            //             Name = "user",
            //             NormalizedName = "USER"
            //         }
            //     }    
            // );

            builder.Entity<Champion>().HasNoKey();

            builder.Entity<VersusUser>()
                .HasKey(u => new { u.UserId, u.VersusId});

            builder.Entity<UserSocket>()
                .HasKey(u => new { u.UserId, u.SocketId});
            

            base.OnModelCreating(builder);
        }

    }
}