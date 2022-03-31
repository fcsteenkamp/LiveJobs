using LiveJobs.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiveJobs
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;

                  var jobs = configuration.GetSection("Jobs").GetChildren().ToList();

                    services.AddHostedService<Worker>();

                    services.AddLiveJob<Job1>(c =>
                    {
                        c.Jobname = "Collect some data 1";
                        c.CronExpression = @"* * * * *";
                    });

                    services.AddLiveJob<Job2>(c =>
                    {
                        c.Jobname = "Collect some data 2";
                        c.TimeZoneInfo = TimeZoneInfo.Local;
                        c.CronExpression = @"* * * * *";
                    });

                    services.AddLiveJob<Job3>(c =>
                    {
                        c.Jobname = "Collect some data 3";
                        c.CronExpression = @"*/5 * * * *";
                    });
                });
    }
}
