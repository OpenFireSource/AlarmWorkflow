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
using System.Data.Objects;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Backend.ServiceContracts.Data
{
    /// <summary>
    /// Provides helper methods that help when working with the Entity Framework.
    /// </summary>
    public static class EntityFrameworkHelper
    {
        #region Constants

        private const string ConnectionStringTemplate = "metadata=res://*/{0}.csdl|res://*/{0}.ssdl|res://*/{0}.msl;provider=MySql.Data.MySqlClient;provider connection string=\"server={1};Port={2};User Id={3};Password={4};database={5};Persist Security Info=True\"";

        #endregion

        #region Methods

        /// <summary>
        /// Creates the database context for a connection to the specified server.
        /// </summary>
        /// <typeparam name="T">The ObjectContext type to create.</typeparam>
        /// <param name="options">The options to use for connection.</param>
        /// <returns>The created database context (derived from <see cref="ObjectContext"/>).</returns>
        public static T CreateContext<T>(ContextCreationOptions options) where T : ObjectContext
        {
            Assertions.AssertNotNull(options, "options");

            string connectionString = string.Format(ConnectionStringTemplate,
                options.EdmxPath,
                options.HostName,
                options.Port,
                options.UserId,
                options.Password,
                options.DatabaseName);

            return (T)Activator.CreateInstance(typeof(T), connectionString);
        }

        /// <summary>
        /// Creates the database context for a connection to the specified server,
        /// by using a custom EDMX path and retrieving the other options from the backend configuration.
        /// </summary>
        /// <typeparam name="T">The ObjectContext type to create.</typeparam>
        /// <param name="edmxPath">The path to the .edmx-file. This path must be relative to the project.
        /// See <see cref="ContextCreationOptions.EdmxPath"/> for further information.</param>
        /// <returns>The created database context (derived from <see cref="ObjectContext"/>).</returns>
        public static T CreateContext<T>(string edmxPath) where T : ObjectContext
        {
            Assertions.AssertNotEmpty(edmxPath, "edmxPath");

            ContextCreationOptions options = ContextCreationOptions.CreateFromSettings();
            options.EdmxPath = edmxPath;
            return CreateContext<T>(options);
        }

        #endregion
    }
}
