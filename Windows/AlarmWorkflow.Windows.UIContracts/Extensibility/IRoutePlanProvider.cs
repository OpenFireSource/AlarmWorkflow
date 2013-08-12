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

using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Windows.UIContracts.Extensibility
{
    /// <summary>
    /// Defines a means for a type that is able to provide route images from a predefined start (site) to destination (operation).
    /// </summary>
    public interface IRoutePlanProvider
    {
        /// <summary>
        /// Creates a requested for the route image provider and returns the image contents of the route image.
        /// </summary>
        /// <param name="start">The start location.</param>
        /// <param name="destination">The destination location (from the Operation).</param>
        /// <returns>The image contents of the route image. -or- null, if no route image could be retrieved (no connection, invalid addresses etc.).</returns>
        byte[] GetRouteImage(PropertyLocation start, PropertyLocation destination);
    }
}