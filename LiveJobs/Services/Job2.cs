using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LiveJobs.Services
{
    public class Job2 : BaseJob
    {
        private readonly ILogger<Job2> _logger;

        public Job2(IScheduleConfig<Job2> config, ILogger<Job2> logger)
            : base(config.Jobname, config.CronExpression, config.TimeZoneInfo,logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Do some custom logic on start of job
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Job: {_Jobname} starts.");
            return base.StartAsync(cancellationToken);
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} Job {_Jobname} is working.");

            Thread.Sleep(30000);

            _logger.LogWarning($"{DateTime.Now:hh:mm:ss} Job {_Jobname} is done working.");

            return Task.CompletedTask;
        }
        /// <summary>
        /// Do some special logic on stopping of job
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Job: {_Jobname} is stopping.");
            return base.StopAsync(cancellationToken);
        }
    }
}
