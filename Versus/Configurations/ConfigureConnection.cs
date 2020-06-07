using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Versus.Core.EF;

namespace Versus.Configurations
{
    public static class ConfigureConnection
    {
        public static IServiceCollection AddConnectionProvider
            (this IServiceCollection services, 
            IConfiguration configuration)
        {
            services.AddDbContext<VersusContext>(opt =>
            opt.UseNpgsql(
                "Host=localhost;Port=5432;Database=versus_db;Username=postgres;Password=2423",
                b => b.MigrationsAssembly("Versus")));
            
            

            return services;
        }
    }

    
}
