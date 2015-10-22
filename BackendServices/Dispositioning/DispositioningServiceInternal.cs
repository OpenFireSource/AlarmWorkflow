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
using AlarmWorkflow.Backend.Data;
using AlarmWorkflow.Backend.Data.Types;
using AlarmWorkflow.Backend.ServiceContracts.Core;
using AlarmWorkflow.BackendService.DispositioningContracts;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.BackendService.Dispositioning
{
    class DispositioningServiceInternal : InternalServiceBase, IDispositioningServiceInternal
    {
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
            using (IUnitOfWork work = ServiceProvider.GetService<IDataContextFactory>().Get().Create())
            {
                return work.For<DispositionedResourceData>()
                    .Query
                    .Where(_ => _.OperationId == operationId)
                    .Select(_ => _.EmkResourceId)
                    .ToArray();
            }
        }

        void IDispositioningServiceInternal.Dispatch(int operationId, string emkResourceId)
        {
            lock (SyncRoot)
            {
                using (IUnitOfWork work = ServiceProvider.GetService<IDataContextFactory>().Get().Create())
                {
                    var repository = work.For<DispositionedResourceData>();

                    bool exists = repository.Query.Any(_ => _.OperationId == operationId && _.EmkResourceId == emkResourceId);
                    if (exists)
                    {
                        throw new InvalidOperationException(Properties.Resources.DispatchNotPossibleEntryAlreadyExists);
                    }

                    DispositionedResourceData data = repository.Create();
                    data.OperationId = operationId;
                    data.EmkResourceId = emkResourceId;
                    data.Timestamp = DateTime.Now;

                    repository.Insert(data);

                    work.Commit();
                }
            }

            DispositionEventArgs args = new DispositionEventArgs(operationId, emkResourceId, DispositionEventArgs.ActionType.Dispatch);
            OnDispositionEventArgs(args);
        }

        void IDispositioningServiceInternal.Recall(int operationId, string emkResourceId)
        {
            lock (SyncRoot)
            {
                using (IUnitOfWork work = ServiceProvider.GetService<IDataContextFactory>().Get().Create())
                {
                    var repository = work.For<DispositionedResourceData>();

                    DispositionedResourceData exists = repository.Query.SingleOrDefault(_ => _.OperationId == operationId && _.EmkResourceId == emkResourceId);
                    if (exists == null)
                    {
                        throw new InvalidOperationException(Properties.Resources.RecallNotPossibleEntryDoesNotExist);
                    }

                    repository.Delete(exists);

                    work.Commit();
                }
            }

            DispositionEventArgs args = new DispositionEventArgs(operationId, emkResourceId, DispositionEventArgs.ActionType.Recall);
            OnDispositionEventArgs(args);
        }

        #endregion
    }
}
