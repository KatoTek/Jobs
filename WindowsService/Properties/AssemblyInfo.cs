using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("Jobs.WindowsService")]
[assembly: AssemblyDescription("Service that continuously runs to execute Jobs.Runner")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyCompany("KatoTek")]
[assembly: AssemblyProduct("Jobs")]
[assembly: AssemblyCopyright("Copyright © KatoTek 2016")]
[assembly: AssemblyTrademark("KatoTek®")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: Guid("ce58d549-616e-4b65-8f55-2d602547a8ab")]
[assembly: AssemblyVersion("2.0.16.815")]
[assembly: AssemblyFileVersion("2.0.16.815")]