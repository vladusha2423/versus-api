using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Versus.WebSockets
{
    public static class WebSocketManagerExtensions
    {
        public static IServiceCollection AddWebSocketManager(this IServiceCollection services)
        {
            services.AddTransient<ConnectionManager>();
            services.AddSingleton<MessagesHandler>();

            return services;
        }

        public static IApplicationBuilder MapWebSocketManager(this IApplicationBuilder app, 
                                                              PathString path,
                                                             MessagesHandler handler)
        {
            return app.Map(path, builder => builder.UseMiddleware<WebSocketManagerMiddleware>(handler));
        }
    }
}