using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Quartz;
using Versus.Data.Entities;
using Versus.Messaging.Interfaces;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Versus.Core.EF;

namespace Versus.Statistics
{
    [DisallowConcurrentExecution]
    public class StatisticsJob : IJob
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMobileMessagingClient _mmc;
        
        public StatisticsJob(
            IMobileMessagingClient mmc, 
            IServiceScopeFactory serviceScopeFactory)
        {
            _mmc = mmc;
            _serviceScopeFactory = serviceScopeFactory;
        }
        
        private readonly string sql = "select u.\"UserName\" as UserName, " +
                                      "abs.\"Wins\" + sqs.\"Wins\" + pls.\"Wins\" + phs.\"Wins\" as Wins, " +
                                      "u.\"Country\" as Country, " +
                                      "0 as Rate, " +
                                      "u.\"Email\" as Email " +
                                      "from public.\"AspNetUsers\" as u " +
                                      "join public.\"Exercises\" as ex on ex.\"UserId\" = u.\"Id\" " +
                                      "join public.\"Exercise\" as abs on abs.\"Id\" = ex.\"AbsId\" " +
                                      "join public.\"Exercise\" as sqs on sqs.\"Id\" = ex.\"SquatsId\" " +
                                      "join public.\"Exercise\" as pls on pls.\"Id\" = ex.\"PullUpsId\" " +
                                      "join public.\"Exercise\" as phs on phs.\"Id\" = ex.\"PushUpsId\" " +
                                      "where u.\"IsVip\" = true " +
                                      "order by Wins desc limit 3; ";

        public async Task Execute(IJobExecutionContext context)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<VersusContext>();
            var champs = dbContext.Champions.FromSqlRaw(sql).ToList();
            using MailMessage message = new MailMessage("dvv27072000@gmail.com", "dvv2423@ya.ru");
            message.Subject = "Versus Статистика";
            message.IsBodyHtml = true;
            message.Body = String.Format(
                @"<table>
                    <tr>
                        <th>Rate</th>
                        <th>UserName</th>
                        <th>Email</th>
                        <th>Wins</th>
                    </tr>
                    <tr>
                        <td>1</td>
                        <td>{0}</td>
                        <td>{1}</td>
                        <td>{2}</td>
                    </tr>
                    <tr>
                        <td>2</td>
                        <td>{3}</td>
                        <td>{4}</td>
                        <td>{5}</td>
                    </tr>
                    <tr>
                        <td>3</td>
                        <td>{6}</td>
                        <td>{7}</td>
                        <td>{8}</td>
                    </tr>
                </table>", champs[0].UserName, champs[0].Email, champs[0].Wins,
                           champs[1].UserName, champs[1].Email, champs[1].Wins,
                           champs[2].UserName, champs[2].Email, champs[2].Wins);
            Console.WriteLine("SEND EMAIL MESSAGE");
            using SmtpClient client = new SmtpClient
            {
                EnableSsl = true,
                Host = "smtp.gmail.com",
                Port = 587,
                Credentials = new NetworkCredential("dvv27072000@gmail.com", "2423vladusha")
            };
            await client.SendMailAsync(message);
        }
    }
}