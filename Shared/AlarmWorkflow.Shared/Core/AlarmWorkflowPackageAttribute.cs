using System;
using System.Linq;
using System.Reflection;

namespace AlarmWorkflow.Shared.Core
{
    /// <summary>
    /// Specifies the package information for packages used in the AlarmWorfklow-ecosystem.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
    public sealed class AlarmWorkflowPackageAttribute : Attribute
    {
        // Intentionally empty currently. May be extended one day to specify loading/inlining information to skip inlining for certain packages.

        #region Methods

        /// <summary>
        /// Searches for the <see cref="T:AlarmWorkflowPackageAttribute"/> defined in the given assembly.
        /// </summary>
        /// <param name="assembly">The assembly to search in.</param>
        /// <param name="attribute">If the attribute was defined in the assembly, contains it as the result.</param>
        /// <returns>Whether or not the attribute was specified in the assembly.</returns>
        public static bool TryGetAttribute(Assembly assembly, out AlarmWorkflowPackageAttribute attribute)
        {
            Assertions.AssertNotNull(assembly, "assembly");

            AlarmWorkflowPackageAttribute[] attributes = (AlarmWorkflowPackageAttribute[])assembly.GetCustomAttributes(typeof(AlarmWorkflowPackageAttribute), false);

            attribute = attributes.FirstOrDefault();
            return attribute != null;
        }

        #endregion
    }
}
