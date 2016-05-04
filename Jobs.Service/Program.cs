using System.ServiceProcess;
using static System.ServiceProcess.ServiceBase;

namespace Jobs.Service
{
    static class Program
    {
        #region methods

        static void Main() => Run(new ServiceBase[] { new Service() });

        #endregion
    }
}
