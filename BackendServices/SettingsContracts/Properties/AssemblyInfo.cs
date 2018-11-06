using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AlarmWorkflow.Shared.Core;

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("e9b95cb9-3d9d-42f8-82de-652a394acf54")]

[assembly: InternalsVisibleTo("AlarmWorkflow.BackendService.Settings")]

[assembly: AlarmWorkflowPackage()]