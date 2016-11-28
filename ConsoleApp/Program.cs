using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Jobs.WindowsService;
using static System.Console;

namespace Jobs.ConsoleApp
{
    class Program
    {
        #region fields

        static readonly ManualResetEvent QuitEvent = new ManualResetEvent(false);

        #endregion

        #region methods

        [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
        static void Main()
        {
            using (var service = new Service())
            {
                var cancelKeyPressEventHandler = (ConsoleCancelEventHandler)((sender, e) =>
                                                                             {
                                                                                 e.Cancel = true;
                                                                                 service.Cancel();
                                                                             });

                var serviceCancelledEventHandler = (EventHandler)((sender, e) => QuitEvent.Set());

                CancelKeyPress += cancelKeyPressEventHandler;
                service.Cancelled += serviceCancelledEventHandler;

                try
                {
                    service.Log += WriteLine;
                    service.Launch();
                    WriteLine("Press Ctrl+C to quit!");
                    QuitEvent.WaitOne();
                }
                finally
                {
                    service.Log -= WriteLine;
                    service.Cancelled -= serviceCancelledEventHandler;
                    CancelKeyPress -= cancelKeyPressEventHandler;
                }
            }
        }

        #endregion
    }
}
