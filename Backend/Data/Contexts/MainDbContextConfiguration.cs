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

using System.Data.Entity;
using MySql.Data.Entity;
using MySql.Data.MySqlClient;

namespace AlarmWorkflow.Backend.Data.Contexts
{
    /// <summary>
    /// Implementation of <see cref="DbConfiguration"/> to set up the required classes for the Entity Framework.
    /// </summary>
    class MainDbContextConfiguration : DbConfiguration
    {
        #region Constructors

        internal MainDbContextConfiguration()
        {
            SetDefaultConnectionFactory(new MySqlConnectionFactory());

            SetProviderFactory("MySql.Data.MySqlClient", new MySqlClientFactory());
            SetProviderServices("MySql.Data.MySqlClient", new MySqlProviderServices());
            SetHistoryContext("MySql.Data.MySqlClient", (conn, schema) => new MySqlHistoryContext(conn, schema));
            SetMigrationSqlGenerator("MySql.Data.MySqlClient", () => new MySqlMigrationSqlGenerator());
        }

        #endregion
    }
}
