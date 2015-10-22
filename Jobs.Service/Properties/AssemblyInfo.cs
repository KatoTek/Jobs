using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("Jobs.Service")]
[assembly: AssemblyDescription("Service that continuously runs to execute JobRunner")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyCompany("KatoTek")]
[assembly: AssemblyProduct("Jobs")]
[assembly: AssemblyCopyright("Copyright © KatoTek 2015")]
[assembly: AssemblyTrademark("KatoTek®")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: Guid("ce58d549-616e-4b65-8f55-2d602547a8ab")]
[assembly: AssemblyVersion("1.6.15.814")]
[assembly: AssemblyFileVersion("1.6.15.814")]