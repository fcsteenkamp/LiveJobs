using Cronos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LiveJobs.Services
{
    public abstract class BaseJob : IHostedService, IDisposable
    {

        private System.Timers.Timer _timer;
        private readonly CronExpression _expression;
        private readonly TimeZoneInfo _timeZoneInfo;
        protected readonly string _Jobname;
        private readonly ILogger<BaseJob> _logger;

        protected BaseJob(string jobName,string cronExpression, TimeZoneInfo timeZoneInfo, ILogger<BaseJob> logger)
        {
            _Jobname = jobName;
            _expression = CronExpression.Parse(cronExpression);
            _timeZoneInfo = timeZoneInfo;
            _logger = logger;
        }

        protected virtual async Task ScheduleJob(CancellationToken cancellationToken)
        {
            var next = _expression.GetNextOccurrence(DateTimeOffset.Now, _timeZoneInfo);
            if (next.HasValue)
            {
                var delay = next.Value - DateTimeOffset.Now;
                if (delay.TotalMilliseconds <= 0)   // prevent non-positive values from being passed into Timer
                {
                    await ScheduleJob(cancellationToken);
                }
                _timer = new System.Timers.Timer(delay.TotalMilliseconds);
                _timer.Elapsed += async (sender, args) =>
                {
                    _timer.Dispose();  // reset and dispose timer
                    _timer = null;

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        await DoWork(cancellationToken);
                    }

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        await ScheduleJob(cancellationToken);    // reschedule next
                    }
                };
                _timer.Start();
            }
            await Task.CompletedTask;
        }

        public virtual async Task DoWork(CancellationToken cancellationToken)
        {
            await Task.Delay(5000, cancellationToken);  // do the work
        }

        /// <summary>
        /// Below is all the interface implementations
        /// </summary>
        public void Dispose()
        {
            _timer?.Dispose();
        }

        public virtual async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Job: {_Jobname} starts.");
            await ScheduleJob(cancellationToken);
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Job: {_Jobname} is stopping.");
            _timer?.Stop();
            await Task.CompletedTask;
        }
    }




    public interface IScheduleConfig<T>
    {
        public string Jobname { get; set; }
        string CronExpression { get; set; }
        TimeZoneInfo TimeZoneInfo { get; set; }
    }

    public class ScheduleConfig<T> : IScheduleConfig<T>
    {
        public string Jobname { get; set; }
        public string CronExpression { get; set; }
        public TimeZoneInfo TimeZoneInfo { get; set; }
    }

    public static class ScheduledServiceExtensions
    {
        public static IServiceCollection AddLiveJob<T>(this IServiceCollection services, Action<IScheduleConfig<T>> options) where T : BaseJob
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options), @"Please provide Schedule Configurations.");
            }
            var config = new ScheduleConfig<T>();
            options.Invoke(config);
            if (string.IsNullOrWhiteSpace(config.CronExpression))
            {
                throw new ArgumentNullException(nameof(ScheduleConfig<T>.CronExpression), @"Empty Cron Expression is not allowed.");
            }
            if (config.TimeZoneInfo == null)
            {
                config.TimeZoneInfo = TimeZoneInfo.Local;
            }
            if (string.IsNullOrWhiteSpace(config.Jobname))
            {
                throw new ArgumentNullException(nameof(ScheduleConfig<T>.CronExpression), @"Please supply a job name.");
            }

            services.AddSingleton<IScheduleConfig<T>>(config);
            services.AddHostedService<T>();
            return services;
        }
    }
}
