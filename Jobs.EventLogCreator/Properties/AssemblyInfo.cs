using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("Jobs.EventLogCreator")]
[assembly: AssemblyDescription("Application used to create event logs for job runner instances.")]
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
[assembly: Guid("9a47fab9-974e-4122-9d02-62d9fc77148e")]
[assembly: AssemblyVersion("1.2.15.814")]
[assembly: AssemblyFileVersion("1.2.15.814")]
