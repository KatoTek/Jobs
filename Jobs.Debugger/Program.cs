using static System.Console;

namespace Jobs.Debugger
{
    class Program
    {
        #region methods

        static void Main() => new JobsService().Start();

        #endregion

        #region nested types

        public class JobsService : Service.Service
        {
            #region constructors

            public JobsService()
            {
                OnLog += WriteLine;
            }

            #endregion

            #region methods

            public void Start() => OnStart(new[] { "wait", "debug" });

            #endregion
        }

        #endregion
    }
}
