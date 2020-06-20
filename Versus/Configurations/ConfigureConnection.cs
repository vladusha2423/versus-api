using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
                "Host=188.225.73.49;Port=5432;Database=versus_db;Username=postgres;Password=veRsus2020",
                b => b.MigrationsAssembly("Versus")));
            
            

            return services;
        }
    }

    
}
