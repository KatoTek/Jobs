using System.Collections.Generic;
using Telerik.OpenAccess.Metadata;
using Telerik.OpenAccess.Metadata.Fluent;
using static Telerik.OpenAccess.DataAccessKind;
using static Telerik.OpenAccess.Metadata.NamingSourceStrategy;
using static Telerik.OpenAccess.OptimisticConcurrencyControlStrategy;

namespace Jobs.Scheduler
{
    public class EntitiesModelFluentMetadataSource : FluentMetadataSource
    {
        #region methods

        public MappingConfiguration<Job> GetJobClassConfiguration()
        {
            var configuration = new MappingConfiguration<Job>();
            configuration.MapType(x => new { })
                         .WithConcurencyControl(Changed)
                         .ToTable("js_Jobs");

            return configuration;
        }

        public MappingConfiguration<JobException> GetJobExceptionClassConfiguration()
        {
            var configuration = new MappingConfiguration<JobException>();
            configuration.MapType(x => new { })
                         .WithConcurencyControl(Changed)
                         .ToTable("js_JobExceptions");

            return configuration;
        }

        public MappingConfiguration<JobException> GetJobExceptionMappingConfiguration()
        {
            var configuration = GetJobExceptionClassConfiguration();
            PrepareJobExceptionPropertyConfigurations(configuration);
            PrepareJobExceptionAssociationConfigurations(configuration);

            return configuration;
        }

        public MappingConfiguration<JobInstance> GetJobInstanceClassConfiguration()
        {
            var configuration = new MappingConfiguration<JobInstance>();
            configuration.MapType(x => new { })
                         .WithConcurencyControl(Changed)
                         .ToTable("js_JobInstances");

            return configuration;
        }

        public MappingConfiguration<JobInstance> GetJobInstanceMappingConfiguration()
        {
            var configuration = GetJobInstanceClassConfiguration();
            PrepareJobInstancePropertyConfigurations(configuration);
            PrepareJobInstanceAssociationConfigurations(configuration);

            return configuration;
        }

        public MappingConfiguration<Job> GetJobMappingConfiguration()
        {
            var configuration = GetJobClassConfiguration();
            PrepareJobPropertyConfigurations(configuration);
            PrepareJobAssociationConfigurations(configuration);

            return configuration;
        }

        public MappingConfiguration<JobSchedule> GetJobScheduleClassConfiguration()
        {
            var configuration = new MappingConfiguration<JobSchedule>();
            configuration.MapType(x => new { })
                         .WithConcurencyControl(Changed)
                         .ToTable("js_JobSchedules");

            return configuration;
        }

        public MappingConfiguration<JobSchedule> GetJobScheduleMappingConfiguration()
        {
            var configuration = GetJobScheduleClassConfiguration();
            PrepareJobSchedulePropertyConfigurations(configuration);
            PrepareJobScheduleAssociationConfigurations(configuration);

            return configuration;
        }

        public MappingConfiguration<JobScheduleType> GetJobScheduleTypeClassConfiguration()
        {
            var configuration = new MappingConfiguration<JobScheduleType>();
            configuration.MapType(x => new { })
                         .WithConcurencyControl(Changed)
                         .ToTable("js_JobScheduleTypes");

            return configuration;
        }

        public MappingConfiguration<JobScheduleType> GetJobScheduleTypeMappingConfiguration()
        {
            var configuration = GetJobScheduleTypeClassConfiguration();
            PrepareJobScheduleTypePropertyConfigurations(configuration);
            PrepareJobScheduleTypeAssociationConfigurations(configuration);

            return configuration;
        }

        public void PrepareJobAssociationConfigurations(MappingConfiguration<Job> configuration)
        {
            configuration.HasAssociation(x => x.JobSchedules)
                         .WithOpposite(x => x.Job)
                         .ToColumn("JobId")
                         .HasConstraint((y, x) => x.JobId == y.JobId)
                         .WithDataAccessKind(ReadWrite);
            configuration.HasAssociation(x => x.JobInstances)
                         .WithOpposite(x => x.Job)
                         .ToColumn("JobId")
                         .HasConstraint((y, x) => x.JobId == y.JobId)
                         .WithDataAccessKind(ReadWrite);
        }

        public void PrepareJobExceptionAssociationConfigurations(MappingConfiguration<JobException> configuration)
        {
            configuration.HasAssociation(x => x.JobInstance)
                         .WithOpposite(x => x.JobExceptions)
                         .ToColumn("JobInstanceId")
                         .HasConstraint((x, y) => x.JobInstanceId == y.JobInstanceId)
                         .IsRequired()
                         .WithDataAccessKind(ReadWrite);
            configuration.HasAssociation(x => x.InnerJobException)
                         .ToColumn("ChildId")
                         .HasConstraint((x, y) => x.ChildId == y.JobExceptionId)
                         .WithDataAccessKind(ReadWrite);
        }

        public void PrepareJobExceptionPropertyConfigurations(MappingConfiguration<JobException> configuration)
        {
            configuration.HasProperty(x => x.JobExceptionId)
                         .IsIdentity(KeyGenerator.Autoinc)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("JobExceptionId")
                         .IsNotNullable()
                         .HasColumnType("int")
                         .HasPrecision(0)
                         .HasScale(0);
            configuration.HasProperty(x => x.JobInstanceId)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("JobInstanceId")
                         .IsNotNullable()
                         .HasColumnType("bigint")
                         .HasPrecision(0)
                         .HasScale(0);
            configuration.HasProperty(x => x.ChildId)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("ChildId")
                         .IsNullable()
                         .HasColumnType("int")
                         .HasPrecision(0)
                         .HasScale(0);
            configuration.HasProperty(x => x.Date)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("Date")
                         .IsNullable()
                         .HasColumnType("datetime");
            configuration.HasProperty(x => x.Type)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("Type")
                         .IsNullable()
                         .HasColumnType("nvarchar")
                         .HasLength(500);
            configuration.HasProperty(x => x.Message)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("Message")
                         .IsNullable()
                         .HasColumnType("nvarchar(max)")
                         .HasLength(0);
            configuration.HasProperty(x => x.Source)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("Source")
                         .IsNullable()
                         .HasColumnType("nvarchar")
                         .HasLength(500);
            configuration.HasProperty(x => x.StackTrace)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("StackTrace")
                         .IsNullable()
                         .HasColumnType("nvarchar(max)")
                         .HasLength(0);
            configuration.HasProperty(x => x.HelpLink)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("HelpLink")
                         .IsNullable()
                         .HasColumnType("nvarchar")
                         .HasLength(500);
        }

        public void PrepareJobInstanceAssociationConfigurations(MappingConfiguration<JobInstance> configuration)
        {
            configuration.HasAssociation(x => x.Job)
                         .WithOpposite(x => x.JobInstances)
                         .ToColumn("JobId")
                         .HasConstraint((x, y) => x.JobId == y.JobId)
                         .IsRequired()
                         .WithDataAccessKind(ReadWrite);
            configuration.HasAssociation(x => x.JobExceptions)
                         .WithOpposite(x => x.JobInstance)
                         .ToColumn("JobInstanceId")
                         .HasConstraint((y, x) => x.JobInstanceId == y.JobInstanceId)
                         .WithDataAccessKind(ReadWrite);
        }

        public void PrepareJobInstancePropertyConfigurations(MappingConfiguration<JobInstance> configuration)
        {
            configuration.HasProperty(x => x.JobInstanceId)
                         .IsIdentity(KeyGenerator.Autoinc)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("JobInstanceId")
                         .IsNotNullable()
                         .HasColumnType("bigint")
                         .HasPrecision(0)
                         .HasScale(0);
            configuration.HasProperty(x => x.JobId)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("JobId")
                         .IsNotNullable()
                         .HasColumnType("int")
                         .HasPrecision(0)
                         .HasScale(0);
            configuration.HasProperty(x => x.Start)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("Start")
                         .IsNotNullable()
                         .HasColumnType("datetime")
                         .HasDefaultValue();
            configuration.HasProperty(x => x.End)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("End")
                         .IsNullable()
                         .HasColumnType("datetime");
        }

        public void PrepareJobPropertyConfigurations(MappingConfiguration<Job> configuration)
        {
            configuration.HasProperty(x => x.JobId)
                         .IsIdentity(KeyGenerator.Autoinc)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("JobId")
                         .IsNotNullable()
                         .HasColumnType("int")
                         .HasPrecision(0)
                         .HasScale(0);
            configuration.HasProperty(x => x.JobName)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("JobName")
                         .IsNotNullable()
                         .HasColumnType("nvarchar")
                         .HasLength(1000);
            configuration.HasProperty(x => x.Description)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("Description")
                         .IsNullable()
                         .HasColumnType("nvarchar")
                         .HasLength(1000);
            configuration.HasProperty(x => x.Enabled)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("Enabled")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.LastRun)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("LastRun")
                         .IsNotNullable()
                         .HasColumnType("datetime")
                         .HasDefaultValue();
            configuration.HasProperty(x => x.AppId)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("AppId")
                         .IsNotNullable()
                         .HasColumnType("nvarchar")
                         .HasLength(500)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.AlertAfterTries)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("AlertAfterTries")
                         .IsNotNullable()
                         .HasColumnType("int")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
        }

        public void PrepareJobScheduleAssociationConfigurations(MappingConfiguration<JobSchedule> configuration)
        {
            configuration.HasAssociation(x => x.Job)
                         .WithOpposite(x => x.JobSchedules)
                         .ToColumn("JobId")
                         .HasConstraint((x, y) => x.JobId == y.JobId)
                         .IsRequired()
                         .WithDataAccessKind(ReadWrite);
            configuration.HasAssociation(x => x.JobScheduleType)
                         .WithOpposite(x => x.JobSchedules)
                         .ToColumn("JobScheduleTypeId")
                         .HasConstraint((x, y) => x.JobScheduleTypeId == y.JobScheduleTypeId)
                         .IsRequired()
                         .WithDataAccessKind(ReadWrite);
        }

        public void PrepareJobSchedulePropertyConfigurations(MappingConfiguration<JobSchedule> configuration)
        {
            configuration.HasProperty(x => x.JobScheduleId)
                         .IsIdentity(KeyGenerator.Autoinc)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("JobScheduleId")
                         .IsNotNullable()
                         .HasColumnType("int")
                         .HasPrecision(0)
                         .HasScale(0);
            configuration.HasProperty(x => x.JobId)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("JobId")
                         .IsNotNullable()
                         .HasColumnType("int")
                         .HasPrecision(0)
                         .HasScale(0);
            configuration.HasProperty(x => x.JobScheduleTypeId)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("JobScheduleTypeId")
                         .IsNotNullable()
                         .HasColumnType("int")
                         .HasPrecision(0)
                         .HasScale(0);
            configuration.HasProperty(x => x.RepeatInterval)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("RepeatInterval")
                         .IsNullable()
                         .HasColumnType("int")
                         .HasPrecision(0)
                         .HasScale(0);
            configuration.HasProperty(x => x.ScheduleStart)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("ScheduleStart")
                         .IsNotNullable()
                         .HasColumnType("datetime");
            configuration.HasProperty(x => x.ScheduleEnd)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("ScheduleEnd")
                         .IsNullable()
                         .HasColumnType("datetime");
            configuration.HasProperty(x => x.Sunday)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("Sunday")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.Monday)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("Monday")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.Tuesday)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("Tuesday")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.Wednesday)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("Wednesday")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.Thursday)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("Thursday")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.Friday)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("Friday")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.Saturday)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("Saturday")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.Hour)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("Hour")
                         .IsNotNullable()
                         .HasColumnType("int")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.Minute)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("Minute")
                         .IsNotNullable()
                         .HasColumnType("int")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.AllowedDelayDays)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("AllowedDelayDays")
                         .IsNotNullable()
                         .HasColumnType("int")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.Two)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("Two")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.TwentyTwo)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("TwentyTwo")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.TwentyThree)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("TwentyThree")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.TwentySix)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("TwentySix")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.TwentySeven)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("TwentySeven")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.TwentyOne)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("TwentyOne")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.TwentyNine)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("TwentyNine")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.TwentyFour)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("TwentyFour")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.TwentyFive)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("TwentyFive")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.TwentyEight)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("TwentyEight")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.Twelve)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("Twelve")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.Three)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("Three")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.ThirtyOne)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("ThirtyOne")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.Thirty)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("Thirty")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.Thirteen)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("Thirteen")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.Ten)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("Ten")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.Sixteen)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("Sixteen")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.Six)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("Six")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.Seventeen)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("Seventeen")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.Seven)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("Seven")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.September)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("September")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.One)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("One")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.October)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("October")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.November)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("November")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.Nineteen)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("Nineteen")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.Nine)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("Nine")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.May)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("May")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.March)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("March")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.LastDayOfMonth)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("LastDayOfMonth")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.June)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("June")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.July)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("July")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.January)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("January")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.Fourteen)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("Fourteen")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.Four)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("Four")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.Five)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("Five")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.Fifteen)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("Fifteen")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.February)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("February")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.Eleven)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("Eleven")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.Eighteen)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("Eighteen")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.Eight)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("Eight")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.December)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("December")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.August)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("August")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.April)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("April")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
            configuration.HasProperty(x => x.Twenty)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("Twenty")
                         .IsNotNullable()
                         .HasColumnType("bit")
                         .HasPrecision(0)
                         .HasScale(0)
                         .HasDefaultValue();
        }

        public void PrepareJobScheduleTypeAssociationConfigurations(MappingConfiguration<JobScheduleType> configuration)
        {
            configuration.HasAssociation(x => x.JobSchedules)
                         .WithOpposite(x => x.JobScheduleType)
                         .ToColumn("JobScheduleTypeId")
                         .HasConstraint((y, x) => x.JobScheduleTypeId == y.JobScheduleTypeId)
                         .WithDataAccessKind(ReadWrite);
        }

        public void PrepareJobScheduleTypePropertyConfigurations(MappingConfiguration<JobScheduleType> configuration)
        {
            configuration.HasProperty(x => x.JobScheduleTypeId)
                         .IsIdentity(KeyGenerator.Autoinc)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("JobScheduleTypeId")
                         .IsNotNullable()
                         .HasColumnType("int")
                         .HasPrecision(0)
                         .HasScale(0);
            configuration.HasProperty(x => x.Type)
                         .WithDataAccessKind(ReadWrite)
                         .ToColumn("Type")
                         .IsNotNullable()
                         .HasColumnType("nvarchar")
                         .HasLength(100);
        }

        protected override IList<MappingConfiguration> PrepareMapping()
        {
            var mappingConfigurations = new List<MappingConfiguration>();

            var jobscheduletypeConfiguration = GetJobScheduleTypeMappingConfiguration();
            mappingConfigurations.Add(jobscheduletypeConfiguration);

            var jobscheduleConfiguration = GetJobScheduleMappingConfiguration();
            mappingConfigurations.Add(jobscheduleConfiguration);

            var jobConfiguration = GetJobMappingConfiguration();
            mappingConfigurations.Add(jobConfiguration);

            var jobinstanceConfiguration = GetJobInstanceMappingConfiguration();
            mappingConfigurations.Add(jobinstanceConfiguration);

            var jobexceptionConfiguration = GetJobExceptionMappingConfiguration();
            mappingConfigurations.Add(jobexceptionConfiguration);

            return mappingConfigurations;
        }

        protected override void SetContainerSettings(MetadataContainer container)
        {
            container.Name = "EntitiesModel";
            container.DefaultNamespace = "JobScheduler";
            container.NameGenerator.RemoveLeadingUnderscores = false;
            container.NameGenerator.SourceStrategy = Property;
            container.NameGenerator.RemoveCamelCase = false;
        }

        #endregion
    }
}
