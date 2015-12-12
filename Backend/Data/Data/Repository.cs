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
using System.Linq;
using AlarmWorkflow.Backend.Data.Types;

namespace AlarmWorkflow.Backend.Data
{
    class Repository<TEntity> : IRepository<TEntity> where TEntity : EntityBase, new()
    {
        #region Fields

        private DbContext _context;

        #endregion

        #region Constructors

        internal Repository(DbContext context)
        {
            _context = context;
        }

        #endregion

        #region IRepository<TEntity> Members

        IQueryable<TEntity> IRepository<TEntity>.Query
        {
            get { return _context.Set<TEntity>(); }
        }

        TEntity IRepository<TEntity>.Create()
        {
            return new TEntity();
        }

        TEntity IRepository<TEntity>.Insert(TEntity entity)
        {
            return _context.Set<TEntity>().Add(entity);
        }

        void IRepository<TEntity>.Delete(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
        }

        #endregion
    }
}
