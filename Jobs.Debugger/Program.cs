using System;
using Jobs.WindowsService;

namespace Jobs.ConsoleApp
{
    class Program
    {
        #region methods

        static void Main() => new JobsService().SimulateStart();

        #endregion

        #region nested types

        public class JobsService : Service
        {
            #region methods

            public void SimulateStart() => OnStart(new[] { "wait", "debug" });

            protected override void OnStart(string[] args)
            {
                Log += Console.WriteLine;
                base.OnStart(args);
            }

            protected override void OnStop()
            {
                base.OnStop();
                Log += Console.WriteLine;
            }

            #endregion
        }

        #endregion
    }
}
