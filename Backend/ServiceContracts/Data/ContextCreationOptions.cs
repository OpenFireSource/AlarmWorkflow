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

using AlarmWorkflow.Backend.ServiceContracts.Communication;

namespace AlarmWorkflow.Backend.ServiceContracts.Data
{
    /// <summary>
    /// Specifies options to connect to a MySQL server.
    /// </summary>
    public sealed class ContextCreationOptions
    {
        #region Properties

        /// <summary>
        /// Gets/sets the path to the EDMX file.
        /// See documentation for further information.
        /// </summary>
        /// <remarks>This path is relative to the project directory and without the file extension. Example (without quotes):
        /// 'Data.Entities' to refer to the 'Entities.edmx' file inside the 'Data' subdirectory.</remarks>
        public string EdmxPath { get; set; }
        /// <summary>
        /// Gets/sets the host name of the server to connect to.
        /// </summary>
        public string HostName { get; set; }
        /// <summary>
        /// Gets/sets the port of the server to connect to.
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// Gets/sets the name of the user for logging in.
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// Gets/sets the password of the user.
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Gets/sets the name of the database to connect to.
        /// </summary>
        public string DatabaseName { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextCreationOptions"/> class.
        /// </summary>
        public ContextCreationOptions()
        {

        }

        #endregion

        #region Factory methods

        /// <summary>
        /// Creates the <see cref="ContextCreationOptions"/> and uses the values from the backend configuration.
        /// </summary>
        /// <returns>An instance of <see cref="ContextCreationOptions"/> which uses the values from the backend configuration.</returns>
        public static ContextCreationOptions CreateFromSettings()
        {
            ContextCreationOptions options = new ContextCreationOptions();
            options.HostName = ServiceFactory.BackendConfigurator.Get("Server.DB.HostName");
            options.Port = int.Parse(ServiceFactory.BackendConfigurator.Get("Server.DB.Port"));
            options.UserId = ServiceFactory.BackendConfigurator.Get("Server.DB.UserId");
            options.Password = ServiceFactory.BackendConfigurator.Get("Server.DB.Password");
            options.DatabaseName = ServiceFactory.BackendConfigurator.Get("Server.DB.DatabaseName");
            return options;
        }

        #endregion
    }
}
