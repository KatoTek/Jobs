using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("Jobs.Scheduler")]
[assembly: AssemblyDescription("Exposes the ScheduledJob class used for implementing specific jobs on a schedule")]
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
[assembly: Guid("0fcca896-2d1e-49ce-97f1-8ef4cb5a6f98")]
[assembly: AssemblyVersion("6.1.16.1127")]
[assembly: AssemblyFileVersion("6.1.16.1127")]