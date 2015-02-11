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
using System.Linq;
using AlarmWorkflow.Backend.ServiceContracts.Core;
using AlarmWorkflow.Backend.ServiceContracts.Data;
using AlarmWorkflow.BackendService.Dispositioning.Data;
using AlarmWorkflow.BackendService.DispositioningContracts;

namespace AlarmWorkflow.BackendService.Dispositioning
{
    class DispositioningServiceInternal : InternalServiceBase, IDispositioningServiceInternal
    {
        #region Constants

        private const string EdmxPath = "Data.Entities";

        #endregion

        #region IDispositioningServiceInternal Members

        /// <summary>
        /// Raised if a resource was dispositioned or un-dispositioned.
        /// </summary>
        public event EventHandler<DispositionEventArgs> Dispositioning;

        private void OnDispositionEventArgs(DispositionEventArgs e)
        {
            var copy = Dispositioning;
            if (copy != null)
            {
                copy(this, e);
            }
        }

        string[] IDispositioningServiceInternal.GetDispatchedResources(int operationId)
        {
            lock (SyncRoot)
            {
                using (var entities = EntityFrameworkHelper.CreateContext<DispositioningEntities>(EdmxPath))
                {
                    return entities.DispResources.Where(_ => _.Operation_Id == operationId).Select(_ => _.EmkResourceId).ToArray();
                }
            }
        }

        void IDispositioningServiceInternal.Dispatch(int operationId, string emkResourceId)
        {
            lock (SyncRoot)
            {
                using (var entities = EntityFrameworkHelper.CreateContext<DispositioningEntities>(EdmxPath))
                {
                    bool exists = entities.DispResources.Any(_ => _.Operation_Id == operationId && _.EmkResourceId == emkResourceId);
                    if (exists)
                    {
                        throw new InvalidOperationException(Properties.Resources.DispatchNotPossibleEntryAlreadyExists);
                    }

                    DispResourceData data = new DispResourceData();
                    data.Operation_Id = operationId;
                    data.EmkResourceId = emkResourceId;
                    data.Timestamp = DateTime.Now;

                    entities.DispResources.AddObject(data);
                    entities.SaveChanges();
                }
            }

            DispositionEventArgs args = new DispositionEventArgs(operationId, emkResourceId, DispositionEventArgs.ActionType.Dispatch);
            OnDispositionEventArgs(args);
        }

        void IDispositioningServiceInternal.Recall(int operationId, string emkResourceId)
        {
            lock (SyncRoot)
            {
                using (var entities = EntityFrameworkHelper.CreateContext<DispositioningEntities>(EdmxPath))
                {
                    DispResourceData exists = entities.DispResources.SingleOrDefault(_ => _.Operation_Id == operationId && _.EmkResourceId == emkResourceId);
                    if (exists == null)
                    {
                        throw new InvalidOperationException(Properties.Resources.RecallNotPossibleEntryDoesNotExist);
                    }

                    entities.DispResources.DeleteObject(exists);
                    entities.SaveChanges();
                }
            }

            DispositionEventArgs args = new DispositionEventArgs(operationId, emkResourceId, DispositionEventArgs.ActionType.Recall);
            OnDispositionEventArgs(args);
        }

        #endregion
    }
}
