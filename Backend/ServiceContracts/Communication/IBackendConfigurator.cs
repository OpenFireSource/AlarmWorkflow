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


namespace AlarmWorkflow.Backend.ServiceContracts.Communication
{
    /// <summary>
    /// Defines members to access the backend configuration.
    /// See documentation for further information.
    /// </summary>
    /// <remarks>This interface defines a means to be a facade to access the backend configuration,
    /// which is usually stored as 'backend.config' in the application's directory. It also provides
    /// a way for tests to mock this behavior and inject a different configurator.</remarks>
    public interface IBackendConfigurator
    {
        /// <summary>
        /// Returns the configuration value by its key.
        /// </summary>
        /// <param name="key">The key to access the configuration item.</param>
        /// <returns>The value of the configuration entry by the requested key.
        /// -or- null, if no key with the given name existed.</returns>
        string Get(string key);
    }
}
