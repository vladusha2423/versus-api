using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using Versus.Configurations;
using Versus.Messaging.Interfaces;
using Versus.Messaging.Services;
using Versus.Schedule;
using Versus.WebSockets;

namespace Versus
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services
                .AddConnectionProvider(Configuration)
                .AddIdentity()
                .AddRepositories()
                .ConfigureAuthentication(Configuration);
            
            services.AddSingleton<IMobileMessagingClient, MobileMessagingClient >();
            
            // Add Quartz services
            services.AddSingleton<IJobFactory, SingletonJobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

            // Add our job
            services.AddSingleton<NotificationsJob>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(NotificationsJob),
                // cronExpression: "* 0/1 * * * ? *"));
                cronExpression: "0 0 12 * * ? *"));
            
            services.AddHostedService<QuartzHostedService>();
            
            
            services.AddSingleton<IUserIdProvider, UserIdProvider>();

            
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials()
                            .SetIsOriginAllowed(_ => true);
                    });
            });
            
            services.AddMvc();
            services.AddControllersWithViews(mvcOptions =>
            {
                mvcOptions.EnableEndpointRouting = false;
            }).AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );
            services.AddWebSocketManager();
            
            

        }

        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                
            }
            
            var serviceScopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            var serviceProvider = serviceScopeFactory.CreateScope().ServiceProvider;

            app.UseWebSockets();
            app.MapWebSocketManager("/ws", serviceProvider.GetService<MessagesHandler>());

            app.UseStaticFiles();

            app.UseRouting();
            
            app.UseCors("AllowAll");
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMvc();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            
        }
    }
}