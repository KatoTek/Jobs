using System.ServiceProcess;
using static System.ServiceProcess.ServiceBase;

namespace Jobs.Service
{
    internal static class Program
    {
        private static void Main() => Run(new ServiceBase[] { new Service() });
    }
}
