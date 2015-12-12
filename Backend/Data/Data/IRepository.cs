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

using System.Linq;
using AlarmWorkflow.Backend.Data.Types;

namespace AlarmWorkflow.Backend.Data
{
    /// <summary>
    /// Represents a generic repository for a specific entity type.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IRepository<TEntity> where TEntity : EntityBase, new()
    {
        /// <summary>
        /// Returns an IQueryable which can be used to query the data.
        /// </summary>
        IQueryable<TEntity> Query { get; }

        /// <summary>
        /// Creates a new instance of the entity.
        /// This is the same as calling the constructor of the entity type.
        /// </summary>
        /// <returns></returns>
        TEntity Create();
        /// <summary>
        /// Inserts a new entity into the repository.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        TEntity Insert(TEntity entity);
        /// <summary>
        /// Deletes an existing entity from the repository.
        /// </summary>
        /// <param name="entity"></param>
        void Delete(TEntity entity);
    }
}
