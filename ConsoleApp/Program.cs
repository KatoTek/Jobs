using Jobs.WindowsService;
using static System.Console;

namespace Jobs.ConsoleApp
{
    class Program
    {
        #region methods

        static void Main()
        {
            using (var service = new Service())
            {
                service.Log += WriteLine;
                service.Launch();
                WriteLine("Press [Enter] to quit!");
                ReadLine();
                service.Break();
                service.Log -= WriteLine;
            }
        }

        #endregion
    }
}
