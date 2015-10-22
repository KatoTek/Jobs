using System.Linq;
using Telerik.OpenAccess;

namespace Jobs.Scheduler
{
    public interface IEntitiesModelUnitOfWork : IUnitOfWork
    {
        IQueryable<JobException> JobExceptions { get; }
        IQueryable<JobInstance> JobInstances { get; }
        IQueryable<Job> Jobs { get; }
        IQueryable<JobSchedule> JobSchedules { get; }
        IQueryable<JobScheduleType> JobScheduleTypes { get; }
    }
}
