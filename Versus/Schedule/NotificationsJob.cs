using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Quartz;
using Versus.Data.Entities;
using Versus.Messaging.Interfaces;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Versus.Core.EF;

namespace Versus.Schedule
{
    [DisallowConcurrentExecution]
    public class NotificationsJob : IJob
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMobileMessagingClient _mmc;
        
        public NotificationsJob(
            IMobileMessagingClient mmc, 
            IServiceScopeFactory serviceScopeFactory)
        {
            _mmc = mmc;
            _serviceScopeFactory = serviceScopeFactory;
        }

        private async Task SendNotification(string token, Guid userId)
        {
            await  _mmc.SendNotification(
                    token,
                    "Versus",
                    await ChoosePhrase(userId)
                );
        }

        private async Task<string> ChoosePhrase(Guid userId)
        {
            var phrases = new List<string>
            {
                "Первые в мире спортивные соревнования через интернет! Тренируйся и зарабатывай! 👍",
                "The world's first online sports competitions! Train and earn!",
                "Лучшие спортсмены уже тренируются и зарабатывают на VERSUS! Присоединяйся!👍",
                "The best athletes are already training and earning on VERSUS! Join us!",
                "Здесь можно не только привести себя в отличную форму, но и заработать денег!👍",
                "Here you can not only put yourself in great shape, but also earn money!"
            };
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var rnd = new Random();
                var context = scope.ServiceProvider.GetService<VersusContext>();
                var settings = await context.Settings.FirstOrDefaultAsync(s => s.UserId == userId);
                return phrases[rnd.Next(1, 3) * (settings.Language ? 2 : 1) - 1];
            }
        }
        
        public async Task Execute(IJobExecutionContext context)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetService<UserManager<User>>();
                var users = userManager.Users
                    .Include(u => u.Settings)
                    .ThenInclude(s => s.Notifications)
                    .Where(u => u.Token != null 
                                && u.Token.Length > 100)
                    .ToList();
                if (DateTime.Now.DayOfWeek == DayOfWeek.Monday)
                    foreach (var user in users)
                        if(user.Settings.Notifications.Mon)
                            await SendNotification(user.Token, user.Id);
                if (DateTime.Now.DayOfWeek == DayOfWeek.Tuesday)
                    foreach (var user in users)
                        if(user.Settings.Notifications.Mon)
                            await SendNotification(user.Token, user.Id);
                if (DateTime.Now.DayOfWeek == DayOfWeek.Wednesday)
                    foreach (var user in users)
                        if(user.Settings.Notifications.Wed)
                            await SendNotification(user.Token, user.Id);
                if (DateTime.Now.DayOfWeek == DayOfWeek.Thursday)
                    foreach (var user in users)
                        if(user.Settings.Notifications.Thu)
                            await SendNotification(user.Token, user.Id);
                if (DateTime.Now.DayOfWeek == DayOfWeek.Friday)
                    foreach (var user in users)
                        if(user.Settings.Notifications.Fri)
                            await SendNotification(user.Token, user.Id);
                if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
                    foreach (var user in users)
                        if(user.Settings.Notifications.Sat)
                            await SendNotification(user.Token, user.Id);
                if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                    foreach (var user in users)
                        if(user.Settings.Notifications.Sun)
                            await SendNotification(user.Token, user.Id);
            }
            
        }
    }
}