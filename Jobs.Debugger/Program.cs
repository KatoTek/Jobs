using static System.Console;

namespace Jobs.Debugger
{
    class Program
    {
        #region methods

        static void Main() => new JobsService().SimulateStart();

        #endregion

        #region nested types

        public class JobsService : Service.Service
        {
            #region methods

            public void SimulateStart() => OnStart(new[] { "wait", "debug" });

            protected override void OnStart(string[] args)
            {
                Log += WriteLine;
                base.OnStart(args);
            }

            protected override void OnStop()
            {
                base.OnStop();
                Log += WriteLine;
            }

            #endregion
        }

        #endregion
    }
}
