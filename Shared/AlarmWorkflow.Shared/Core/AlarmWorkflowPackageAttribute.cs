// This file is part of AlarmWorkflow.
// 
// AlarmWorkflow is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// AlarmWorkflow is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with AlarmWorkflow.  If not, see <http://www.gnu.org/licenses/>.

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