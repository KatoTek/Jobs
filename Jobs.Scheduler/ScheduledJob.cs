﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Jobs.Runner;
using Jobs.Scheduler.Exceptions;
using Jobs.Scheduler.Extensions;
using static System.Configuration.ConfigurationManager;
using static System.DayOfWeek;
using static System.Enum;
using static Jobs.Scheduler.ScheduledType;

namespace Jobs.Scheduler
{
    public abstract class ScheduledJob : Runner.Job
    {
        private static readonly object JobSchedulerModelLocker = new object();
        private bool _disposed;
        private Job _job;
        private JobInstance _jobInstance;
        private EntitiesModel _model;
        private bool _runnerIgnoresExceptions;
        private DateTime? _time;

        protected ScheduledJob()
        {
            ExceptionThrown += RecordException;
        }

        public long JobInstanceId
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException("ScheduledJob");

                return _jobInstance?.JobInstanceId ?? 0;
            }
        }

        protected abstract string JobSchedulerAppId { get; }
        protected virtual string JobSchedulerJobName => GetType().FullName;
        protected override bool RunnerIgnoresExceptions => _runnerIgnoresExceptions;

        private Job Job
        {
            get
            {
                if (_job != null)
                    return _job;

                lock (JobSchedulerModelLocker)
                {
                    try
                    {
                        _job = Model.Jobs.First(job => job.JobName == JobSchedulerJobName && job.AppId == JobSchedulerAppId);
                    }
                    catch (Exception exception)
                    {
                        throw new JobNotFoundException(JobSchedulerJobName, Model.Connection.ConnectionString, exception);
                    }
                }

                return _job;
            }
        }

        private EntitiesModel Model
        {
            get
            {
                if (_model != null)
                    return _model;

                ConnectionStringSettings connection;
                try
                {
                    connection = ConnectionStrings["JobSchedulerConnection"];
                }
                catch (Exception exception)
                {
                    throw new ConnectionStringNotFoundException(exception);
                }
                _model = new EntitiesModel(connection.ConnectionString);

                return _model;
            }
        }

        private DateTime Time
        {
            get
            {
                if (_time.HasValue)
                    return _time.Value;

                lock (JobSchedulerModelLocker)
                {
                    _time = Model.GetDateTime();
                }

                return _time.Value;
            }
        }

        public override void Run(bool forceRun)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(ScheduledJob));

            if (!forceRun && !ValidateSchedule())
                return;

            CreateJobInstance();
            try
            {
                ScheduledWork();
                lock (JobSchedulerModelLocker)
                {
                    Job.LastRun = Model.GetDateTime();
                    Model.SaveChanges();
                }
            }
            catch (Exception exception)
            {
                lock (JobSchedulerModelLocker)
                {
                    Model.FlushChanges();
                    _runnerIgnoresExceptions = (from jobInstance in Model.JobInstances where jobInstance.JobId == Job.JobId && jobInstance.Start > Job.LastRun select jobInstance).Count() < Job.AlertAfterTries;
                }

                HandleException(exception);
            }
            finally
            {
                CloseJobInstance();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_model != null)
                    {
                        _model.Dispose();
                        _model = null;
                    }

                    _disposed = true;
                }

                ExceptionThrown -= RecordException;
            }

            base.Dispose(disposing);
        }

        protected override JobExceptionThrownEventArguments GetJobExceptionThrownEventArgument(Exception exception)
            => new ScheduledJobExceptionThrownEventArguments { Exception = exception, RunnerIgnoresExceptions = RunnerIgnoresExceptions };

        protected abstract void ScheduledWork();

        private void CloseJobInstance()
        {
            lock (JobSchedulerModelLocker)
            {
                _jobInstance.End = Model.GetDateTime();
                Model.SaveChanges();
            }
        }

        private void CreateJobInstance()
        {
            _jobInstance = new JobInstance { JobId = Job.JobId, Start = Time };
            lock (JobSchedulerModelLocker)
            {
                Model.Add(_jobInstance);
                Model.SaveChanges();
            }
        }

        private static bool DayOfWeekQualifies(JobSchedule schedule, DateTime datetime)
        {
            switch (datetime.DayOfWeek)
            {
                case Sunday:
                    return schedule.Sunday;

                case Monday:
                    return schedule.Monday;

                case Tuesday:
                    return schedule.Tuesday;

                case Wednesday:
                    return schedule.Wednesday;

                case Thursday:
                    return schedule.Thursday;

                case Friday:
                    return schedule.Friday;

                case Saturday:
                    return schedule.Saturday;

                default:
                    return false;
            }
        }

        private static bool DayQualifies(JobSchedule schedule, DateTime datetime)
        {
            if (schedule.LastDayOfMonth)
            {
                var endOfMonth = new DateTime(datetime.Year, datetime.Month, DateTime.DaysInMonth(datetime.Year, datetime.Month));
                if (datetime.Day == endOfMonth.Day)
                    return true;
            }

            switch (datetime.Day)
            {
                case 1:
                    return schedule.One;
                case 2:
                    return schedule.Two;
                case 3:
                    return schedule.Three;
                case 4:
                    return schedule.Four;
                case 5:
                    return schedule.Five;
                case 6:
                    return schedule.Six;
                case 7:
                    return schedule.Seven;
                case 8:
                    return schedule.Eight;
                case 9:
                    return schedule.Nine;
                case 10:
                    return schedule.Ten;
                case 11:
                    return schedule.Eleven;
                case 12:
                    return schedule.Twelve;
                case 13:
                    return schedule.Thirteen;
                case 14:
                    return schedule.Fourteen;
                case 15:
                    return schedule.Fifteen;
                case 16:
                    return schedule.Sixteen;
                case 17:
                    return schedule.Seventeen;
                case 18:
                    return schedule.Eighteen;
                case 19:
                    return schedule.Nineteen;
                case 20:
                    return schedule.Twenty;
                case 21:
                    return schedule.TwentyOne;
                case 22:
                    return schedule.TwentyTwo;
                case 23:
                    return schedule.TwentyThree;
                case 24:
                    return schedule.TwentyFour;
                case 25:
                    return schedule.TwentyFive;
                case 26:
                    return schedule.TwentySix;
                case 27:
                    return schedule.TwentySeven;
                case 28:
                    return schedule.TwentyEight;
                case 29:
                    return schedule.TwentyNine;
                case 30:
                    return schedule.Thirty;
                case 31:
                    return schedule.ThirtyOne;
                default:
                    return false;
            }
        }

        private IEnumerable<DateTime> GetScheduledDateTimes(JobSchedule schedule)
        {
            var scheduledDateTimes = new List<DateTime>();
            for (var counter = 0; counter > (0 - schedule.AllowedDelayDays); counter--)
            {
                ScheduledType type;
                if (!TryParse(schedule.JobScheduleType.Type, out type))
                    continue;

                var date = Time.AddDays(counter);
                switch (type)
                {
                    case Daily:
                        if (DayOfWeekQualifies(schedule, date))
                            scheduledDateTimes.Add(new DateTime(date.Year, date.Month, date.Day, schedule.Hour, schedule.Minute, 0));
                        break;
                    case Monthly:
                        if (MonthQualifiers(schedule, date) && DayQualifies(schedule, date))
                            scheduledDateTimes.Add(new DateTime(date.Year, date.Month, date.Day, schedule.Hour, schedule.Minute, 0));
                        break;
                    case Minutely:
                        break;
                    case Hourly:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return scheduledDateTimes.Where(dateTime => dateTime <= Time).ToList();
        }

        private int LogException(Exception exception)
        {
            int? childId = null;
            if (exception.InnerException != null)
                childId = LogException(exception.InnerException);

            var jobException = new JobException
                               {
                                   JobInstanceId = JobInstanceId,
                                   ChildId = childId,
                                   Date = DateTime.Now,
                                   Type = exception.GetType().Name,
                                   Message = exception.Message,
                                   Source = exception.Source,
                                   StackTrace = exception.StackTrace,
                                   HelpLink = exception.HelpLink
                               };
            lock (JobSchedulerModelLocker)
            {
                Model.Add(jobException);
                Model.SaveChanges();
            }

            return jobException.JobExceptionId;
        }

        private static bool MonthQualifiers(JobSchedule schedule, DateTime datetime)
        {
            switch (datetime.Month)
            {
                case 1:
                    return schedule.January;
                case 2:
                    return schedule.February;
                case 3:
                    return schedule.March;
                case 4:
                    return schedule.April;
                case 5:
                    return schedule.May;
                case 6:
                    return schedule.June;
                case 7:
                    return schedule.July;
                case 8:
                    return schedule.August;
                case 9:
                    return schedule.September;
                case 10:
                    return schedule.October;
                case 11:
                    return schedule.November;
                case 12:
                    return schedule.December;
                default:
                    return false;
            }
        }

        private void RecordException(object sender, JobExceptionThrownEventArguments args)
        {
            var scheduledJobExceptionThrownEventArguments = args as ScheduledJobExceptionThrownEventArguments;
            if (scheduledJobExceptionThrownEventArguments != null)
                scheduledJobExceptionThrownEventArguments.JobExceptionId = LogException(args.Exception);
        }

        private bool ValidateSchedule()
        {
            if (!Job.Enabled)
                return false;

            foreach (var jobSchedule in Job.JobSchedules.Where(jobSchedule => (jobSchedule.ScheduleStart <= Time) && ((!jobSchedule.ScheduleEnd.HasValue) || (jobSchedule.ScheduleEnd.Value >= Time))))
            {
                ScheduledType type;

                if (!TryParse(jobSchedule.JobScheduleType.Type, out type))
                    continue;

                switch (type)
                {
                    case Minutely:
                        if ((Time - Job.LastRun).Minutes >= jobSchedule.RepeatInterval)
                            return true;
                        break;
                    case Hourly:
                        if ((Time - Job.LastRun).Hours >= jobSchedule.RepeatInterval)
                            return true;
                        break;
                    case Daily:
                    case Monthly:
                        if (GetScheduledDateTimes(jobSchedule).Any(dateTime => dateTime > Job.LastRun))
                            return true;
                        break;
                    default:
                        return false;
                }
            }

            return false;
        }
    }
}
