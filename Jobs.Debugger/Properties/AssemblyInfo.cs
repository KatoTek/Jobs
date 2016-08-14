using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("Jobs.ConsoleApp")]
[assembly: AssemblyDescription("Console app that can be used to debug Jobs.WindowsService")]
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
[assembly: Guid("d33b834b-1e1d-4058-8ad2-77dd3904cc07")]
[assembly: AssemblyVersion("1.4.16.814")]
[assembly: AssemblyFileVersion("1.4.16.814")]