using System;
using static System.Environment;

namespace Jobs.Scheduler.Exceptions
{
    public class JobNotFoundException : Exception
    {
        public JobNotFoundException(string jobname, string connectionString)
            : base($"A job by the name of \"{jobname}\" was not found in the job scheduler data source \"{connectionString}\".") {}

        public JobNotFoundException(string jobname, string connectionString, string message)
            : base($"A job by the name of \"{jobname}\" was not found in the job scheduler data source \"{connectionString}\".{NewLine}{NewLine}{message}") {}

        public JobNotFoundException(string jobname, string connectionString, string message, Exception innerException)
            : base($"A job by the name of \"{jobname}\" was not found in the job scheduler data source \"{connectionString}\".{NewLine}{NewLine}{message}", innerException) {}

        public JobNotFoundException(string jobname, string connectionString, Exception innerException)
            : base($"A job by the name of \"{jobname}\" was not found in the job scheduler data source \"{connectionString}\".", innerException) {}
    }
}
