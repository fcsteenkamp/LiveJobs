using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LiveJobs.Services
{
    public class Job1 : BaseJob
    {
        private readonly ILogger<Job1> _logger;

        public Job1(IScheduleConfig<Job1> config, ILogger<Job1> logger)
            : base(config.Jobname, config.CronExpression, config.TimeZoneInfo,logger)
        {
            _logger = logger;
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} Job {_Jobname} is working.");

            Thread.Sleep(20000);

            _logger.LogWarning($"{DateTime.Now:hh:mm:ss} Job {_Jobname} is done working.");

            return Task.CompletedTask;

        }
    }
}
