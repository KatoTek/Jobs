using System;
using Telerik.OpenAccess;

namespace Jobs.Scheduler.Extensions
{
    /// <summary>
    /// Extension methods for OpenAccessContext types
    /// </summary>
    public static class DataAccessContextExtensions
    {
        /// <summary>
        /// Gets the date time from datasource
        /// </summary>
        /// <param name="context">The OpenAccessContext object</param>
        /// <returns>Returns a DateTime object from the datasource.</returns>
        public static DateTime GetDateTime(this OpenAccessContext context) => context.ExecuteScalar<DateTime>("select getdate()");
    }
}
