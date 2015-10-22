using System;
using System.Linq;
using Jobs.Service;
using JobsService;

namespace JobsServiceDebugger
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            new JobsService().Start();
        }

        public class JobsService : Service
        {
            public JobsService()
            {
                this.ServiceName = base.ServiceName;

                this.OnLog += Console.WriteLine;
            }

            public void Start()
            {
                this.OnStart(new string[]
                             {
                                 "wait",
                                 "debug"
                             });
            }
        }
    }
}