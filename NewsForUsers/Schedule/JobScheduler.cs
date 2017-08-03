using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewsForUsers.Schedule.Jobs
{
    public class JobScheduler
    {
        public static void Start()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<EntityJob>().Build();

            ITrigger trigger = TriggerBuilder.Create()
               .WithDailyTimeIntervalSchedule
               (s =>
                    s.WithIntervalInHours(6)
                    .OnEveryDay()
                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(0, 0))
               )
               .Build();

            scheduler.ScheduleJob(job, trigger);
        }
    }
}