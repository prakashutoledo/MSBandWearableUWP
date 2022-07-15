/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("IDEASLabUT.MSBandWearable.NTPSync")]
[assembly: AssemblyDescription("A NTP synchronization library for MSBandWearable application")]
[assembly: AssemblyCompany("IDEASLab @ University of Toledo")]
[assembly: AssemblyProduct("IDEASLabUT.MSBandWearable.NTPSync")]
[assembly: AssemblyCopyright("Copyright ©  2022 IDEASLab @ University of Toledo")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: InternalsVisibleTo("IDEASLabUT.MSBandWearable.WearableCore")]
[assembly: InternalsVisibleTo("IDEASLabUT.MSBandWearable.Common")]
[assembly: InternalsVisibleTo("IDEASLabUT.MSBandWearable.Test")]

[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("9ad8b682-f299-42e4-8485-901371d88963")]

[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
