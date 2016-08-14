using System.Linq;
using Telerik.OpenAccess;
using Telerik.OpenAccess.Metadata;

namespace Jobs.Scheduler
{
    public class EntitiesModel : OpenAccessContext, IEntitiesModelUnitOfWork
    {
        #region fields

        const string CONNECTION_STRING_NAME = "JobSchedulerConnection";
        static readonly BackendConfiguration Backend = GetBackendConfiguration();
        static readonly MetadataSource MetadataSource = new EntitiesModelFluentMetadataSource();

        #endregion

        #region constructors

        public EntitiesModel()
            : base(CONNECTION_STRING_NAME, Backend, MetadataSource) {}

        public EntitiesModel(string connection)
            : base(connection, Backend, MetadataSource) {}

        public EntitiesModel(BackendConfiguration backendConfiguration)
            : base(CONNECTION_STRING_NAME, backendConfiguration, MetadataSource) {}

        public EntitiesModel(string connection, MetadataSource metadataSource)
            : base(connection, Backend, metadataSource) {}

        public EntitiesModel(string connection, BackendConfiguration backendConfiguration, MetadataSource metadataSource)
            : base(connection, backendConfiguration, metadataSource) {}

        #endregion

        #region properties

        public IQueryable<JobException> JobExceptions => GetAll<JobException>();
        public IQueryable<JobInstance> JobInstances => GetAll<JobInstance>();
        public IQueryable<Job> Jobs => GetAll<Job>();
        public IQueryable<JobSchedule> JobSchedules => GetAll<JobSchedule>();
        public IQueryable<JobScheduleType> JobScheduleTypes => GetAll<JobScheduleType>();

        #endregion

        #region methods

        public static BackendConfiguration GetBackendConfiguration()
        {
            var backend = new BackendConfiguration { Backend = "MsSql", ProviderName = "System.Data.SqlClient" };
            backend.Logging.MetricStoreSnapshotInterval = 0;
            backend.Runtime.SupportConcurrentThreadsInScope = true;

            return backend;
        }

        #endregion
    }
}
