using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LiveJobs.Services
{
    public class Job3 : BaseJob
    {
        private readonly ILogger<Job3> _logger;

        public Job3(IScheduleConfig<Job3> config, ILogger<Job3> logger)
            : base(config.Jobname, config.CronExpression, config.TimeZoneInfo,logger)
        {
            _logger = logger;
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} Job {_Jobname} is working.");

            Thread.Sleep(10000);

            _logger.LogWarning($"{DateTime.Now:hh:mm:ss} Job {_Jobname} is done working.");

            return Task.CompletedTask;

        }
    }
}
