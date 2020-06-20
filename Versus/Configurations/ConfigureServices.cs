using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using Versus.Auth.Interfaces;
using Versus.Auth.Services;
using Versus.Core.EF;
using Versus.Data.Entities;

namespace Versus.Configurations
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddIdentity(
            this IServiceCollection services)
        {
            services.AddIdentity<User, IdentityRole<Guid>>(o =>
            {
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireLowercase = false;
            })
                .AddEntityFrameworkStores<VersusContext>();

            return services;
        }

        public static IServiceCollection AddRepositories(
            this IServiceCollection services)
        {
            services
                .AddTransient<IJwtGenerator, JwtGenerator>()
                .AddTransient<IAuthService, AuthService>();

            return services;
        }


    }
}
