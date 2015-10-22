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
using System.Collections.Concurrent;
using System.Data.Common;
using System.Data.Entity;
using System.Diagnostics;
using AlarmWorkflow.Shared.Core;
using MySql.Data.MySqlClient;

namespace AlarmWorkflow.Backend.Data.Contexts
{
    [DbConfigurationType(typeof(MainDbContextConfiguration))]
    class MainDbContext : DbContext, IUnitOfWork
    {
        #region Constants

        /// <summary>
        /// Represents the fallback connection string, which is used when the Backend.config wasn't found.
        /// Ideally, this should only ever be the case when the EF processes the context.
        /// </summary>
        private const string FallbackConnectionString = "server=127.0.0.1;Port=3306;User Id=root;Password=;database=AlarmWorkflow";

        #endregion

        #region Fields

        private readonly ConcurrentDictionary<string, object> _workCache;

        #endregion

        #region Constructors

        static MainDbContext()
        {
            Database.SetInitializer(new MainDbContextInitializer());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainDbContext"/> class
        /// creates a connection to either the local server (when EF uses it) or the configured server (otherwise) and owns it.
        /// </summary>
        public MainDbContext()
            : base(CreateConnection(), true)
        {
            _workCache = new ConcurrentDictionary<string, object>();
        }

        #endregion

        #region Methods

        private static DbConnection CreateConnection()
        {
            /* Hint: The MainDbContext is currently hard-wired to use MySQL.
             * However, with very little effort it is perfectly possible of using any EF provider (PostgreSQL, SQLite etc.).
             */

            return new MySqlConnection(CreateConnectionString());
        }

        private static string CreateConnectionString()
        {
            try
            {
                return ContextCreationOptions.CreateFromSettings().GetConnectionString();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MainDbContext.CreateConnectionString() failed: {0}", ex.Message);
            }

            return FallbackConnectionString;
        }

        /// <summary>
        /// Overridden.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.AddFromAssembly(GetType().Assembly);

            /* Hint: We have to ignore certain properties from being mapped to the database.
             * In favor of a clean design, we will in future clean up the database schema, i.e. split the lat/lng column in two.
             */
            modelBuilder.ComplexType<PropertyLocation>().Ignore(pl => pl.GeoLatitude);
            modelBuilder.ComplexType<PropertyLocation>().Ignore(pl => pl.GeoLongitude);
        }

        #endregion

        #region IUnitOfWork Members

        IRepository<TEntity> IUnitOfWork.For<TEntity>()
        {
            string key = typeof(TEntity).Name;

            return (IRepository<TEntity>)_workCache.GetOrAdd(key, _ => new Repository<TEntity>(this));
        }

        void IUnitOfWork.Commit()
        {
            SaveChanges();
        }

        #endregion
    }
}
