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

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AlarmWorkflow.Backend.Data.Contexts
{
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

        /// <summary>
        /// Initializes a new instance of the <see cref="MainDbContext"/> class
        /// creates a connection to either the local server (when EF uses it) or the configured server (otherwise) and owns it.
        /// </summary>
        public MainDbContext()
            : base()
        {
            _workCache = new ConcurrentDictionary<string, object>();
        }

        #endregion

        #region Methods

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            
            try
            {
                var options = ContextCreationOptions.CreateFromSettings();
                switch (options.Engine)
                {
                    case ContextCreationOptions.DatabaseEngine.MySQL:
                        optionsBuilder.UseMySql(options.GetMySqlConnectionString());
                        break;
                    case ContextCreationOptions.DatabaseEngine.SQLite:
                        optionsBuilder.UseSqlite(options.GetSQLiteConnectionString());
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MainDbContext.CreateConnectionString() failed: {0}", ex.Message);

                optionsBuilder.UseMySql(FallbackConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
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

        public Task<int> CommitAsync()
        {
            return SaveChangesAsync();
        }

        #endregion
    }
}
