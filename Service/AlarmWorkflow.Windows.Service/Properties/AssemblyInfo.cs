using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AlarmWorkflow.Shared.Core;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("AlarmWorkflow.Windows.Service")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("The AlarmWorkflow-Team")]
[assembly: AssemblyProduct("AlarmWorkflow.Windows.Service")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("e6ce33f4-286a-45e6-acf1-72c1b379ffc5")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("0.9.2.0")]
[assembly: AssemblyFileVersion("0.9.2.0")]

// Make the ServiceConsole-assembly a friend to this assembly so we can use the internal service there.
[assembly: InternalsVisibleTo("AlarmWorkflow.Windows.ServiceConsole")]

[assembly: AlarmWorkflowPackage()]