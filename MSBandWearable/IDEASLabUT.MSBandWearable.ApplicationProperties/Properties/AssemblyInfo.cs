using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("IDEASLabUT.MSBandWearable.ApplicationProperties")]
[assembly: AssemblyDescription("A library for loading application properties for MSBandWearable application")]
[assembly: AssemblyCompany("IDEASLab @ University of Toledo")]
[assembly: AssemblyProduct("IDEASLabUT.MSBandWearable.ApplicationProperties")]
[assembly: AssemblyCopyright("Copyright ©  2022 IDEASLab @ University of Toledo")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: InternalsVisibleTo("IDEASLabUT.MSBandWearable.Common")]
[assembly: InternalsVisibleTo("IDEASLabUT.MSBandWearable.ElasticsearchLogger")]
[assembly: InternalsVisibleTo("IDEASLabUT.MSBandWearable.Test")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

[assembly: ComVisible(false)]


[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

[assembly: Guid("5f07da87-0532-433c-aaea-bfaeeac7f0f8")]
