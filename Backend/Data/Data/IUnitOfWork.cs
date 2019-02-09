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
using AlarmWorkflow.Backend.Data.Types;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AlarmWorkflow.Backend.Data
{
    /// <summary>
    /// Represents a unit of work.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Returns a reference to a repository which can be used to modify the data.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        IRepository<TEntity> For<TEntity>() where TEntity : EntityBase, new();

        DatabaseFacade Database { get; }

        /// <summary>
        /// Commits all changes and persists them to the data context.
        /// </summary>
        void Commit();
    }
}
