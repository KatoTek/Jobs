using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jobs.Runner;
using Jobs.Scheduler.Exceptions;
using Jobs.Scheduler.Extensions;
using static System.Configuration.ConfigurationManager;
using static System.DateTime;
using static System.DayOfWeek;
using static System.Enum;
using static System.Threading.Tasks.Task;
using static Jobs.Scheduler.ScheduledType;

namespace Jobs.Scheduler
{
    public abstract class ScheduledJob : Runner.Job
    {
        #region fields

        static readonly object JobSchedulerModelLocker = new object();
        bool _disposed;
        Job _job;
        JobInstance _jobInstance;
        EntitiesModel _model;
        DateTime? _time;

        #endregion

        #region properties

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

        protected virtual string JobSchedulerJobName => GetType()
            .FullName;

        Job Job
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

        EntitiesModel Model
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

        DateTime Time
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

        #endregion

        #region methods

        public override async Task RunAsync(bool forceRun, CancellationToken cancellationToken)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(ScheduledJob));

            if (cancellationToken.IsCancellationRequested)
            {
                InvokeLog($"\t\t{JobSchedulerJobName} was cancelled");
                cancellationToken.ThrowIfCancellationRequested();
                return;
            }

            if (!forceRun && !ValidateSchedule())
            {
                InvokeLog($"\t\t{JobSchedulerJobName} not scheduled to work at this time");
                return;
            }

            ExceptionThrown += LogException;
            CreateJobInstance();
            InvokeLog($"\t\t{JobSchedulerJobName} started scheduled work");

            try
            {
                await ScheduledWorkAsync(cancellationToken);

                lock (JobSchedulerModelLocker)
                {
                    if (!forceRun)
                        Job.LastRun = Model.GetDateTime();

                    Model.SaveChanges();
                }
            }
            catch (OperationCanceledException)
            {
                InvokeLog($"\t\t{JobSchedulerJobName} was cancelled");
                throw;
            }
            catch (Exception exception)
            {
                lock (JobSchedulerModelLocker)
                {
                    Model.FlushChanges();

                    var jobExceptionId = LogException(exception);

                    if (Model.JobInstances.Count(jobInstance => jobInstance.JobId == Job.JobId && jobInstance.Start > Job.LastRun) < Job.AlertAfterTries)
                    {
                        ExceptionThrown -= LogException;
                        InvokeExceptionThrown(new ScheduledJobExceptionThrownEventArguments { Exception = exception, Job = this, JobExceptionId = jobExceptionId });
                    }
                }
            }
            finally
            {
                CloseJobInstance();
                InvokeLog($"\t\t{JobSchedulerJobName} finished scheduled work");

                ExceptionThrown -= LogException;
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
            }

            base.Dispose(disposing);
        }

        protected virtual async Task ScheduledWorkAsync(CancellationToken cancellationToken) => await Delay(1000, cancellationToken);

        static bool DayOfWeekQualifies(JobSchedule schedule, DateTime datetime)
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

        static bool DayQualifies(JobSchedule schedule, DateTime datetime)
        {
            if (schedule.LastDayOfMonth)
            {
                var endOfMonth = new DateTime(datetime.Year, datetime.Month, DaysInMonth(datetime.Year, datetime.Month));
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

        static bool MonthQualifies(JobSchedule schedule, DateTime datetime)
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

        void CloseJobInstance()
        {
            lock (JobSchedulerModelLocker)
            {
                _jobInstance.End = Model.GetDateTime();
                Model.SaveChanges();
            }
        }

        void CreateJobInstance()
        {
            _jobInstance = new JobInstance { JobId = Job.JobId, Start = Time };
            lock (JobSchedulerModelLocker)
            {
                Model.Add(_jobInstance);
                Model.SaveChanges();
            }
        }

        IEnumerable<DateTime> GetScheduledDateTimes(JobSchedule schedule)
        {
            var scheduledDateTimes = new List<DateTime>();
            for (var counter = 0; counter > 0 - schedule.AllowedDelayDays; counter--)
            {
                ScheduledType type;
                if (!TryParse(schedule.JobScheduleType.Type, out type))
                    continue;

                var date = Time.AddDays(counter);
                switch (type)
                {
                    case Daily:
                        scheduledDateTimes.Add(new DateTime(date.Year, date.Month, date.Day, schedule.Hour, schedule.Minute, 0));
                        break;
                    case Weekly:
                        if (DayOfWeekQualifies(schedule, date))
                            scheduledDateTimes.Add(new DateTime(date.Year, date.Month, date.Day, schedule.Hour, schedule.Minute, 0));
                        break;
                    case Monthly:
                        if (MonthQualifies(schedule, date) && DayQualifies(schedule, date))
                            scheduledDateTimes.Add(new DateTime(date.Year, date.Month, date.Day, schedule.Hour, schedule.Minute, 0));
                        break;
                    case Minutely:
                    case Hourly:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return scheduledDateTimes.Where(dateTime => dateTime <= Time)
                                     .ToList();
        }

        int LogException(Exception exception)
        {
            int? childId = null;
            if (exception.InnerException != null)
                childId = LogException(exception.InnerException);

            var jobException = new JobException
                               {
                                   JobInstanceId = JobInstanceId,
                                   ChildId = childId,
                                   Date = Now,
                                   Type = exception.GetType()
                                                   .Name,
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

        void LogException(object sender, JobExceptionThrownEventArguments args) => LogException(args.Exception);

        bool ValidateSchedule()
        {
            if (!Job.Enabled)
                return false;

            foreach (var jobSchedule in
                Job.JobSchedules.Where(jobSchedule => (jobSchedule.ScheduleStart <= Time) && (!jobSchedule.ScheduleEnd.HasValue || (jobSchedule.ScheduleEnd.Value >= Time))))
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
                    case Weekly:
                    case Monthly:
                        if (GetScheduledDateTimes(jobSchedule)
                            .Any(dateTime => dateTime > Job.LastRun))
                            return true;
                        break;
                    default:
                        return false;
                }
            }

            return false;
        }

        #endregion
    }
}
