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


namespace AlarmWorkflow.Shared.Core
{
    /// <summary>
    /// Defines a property for a type that produces or consumes a different type.
    /// This allows for types that "enrich" other types by wrapping them and providing convenience features,
    /// while still being compliant to the type they are proxying.
    /// </summary>
    /// <typeparam name="TRealType">The underlying type that is proxied.</typeparam>
    public interface IProxyType<TRealType>
    {
        /// <summary>
        /// Gets/sets the value proxied by this instance.
        /// </summary>
        TRealType ProxiedValue { get; set; }
    }
}
