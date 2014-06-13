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

using System.IO;
using AlarmWorkflow.Backend.ServiceContracts.Core;
using AlarmWorkflow.BackendService.FileTransferContracts;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.BackendService.FileTransfer
{
    class FileTransferService : ExposedServiceBase, IFileTransferService
    {
        #region IFileTransferService Members

        string IFileTransferService.GetFileChecksum(string path)
        {
            return this.ServiceProvider.GetService<IFileTransferServiceInternal>().GetFileChecksum(path);
        }

        Stream IFileTransferService.GetFileStream(string path)
        {
            return this.ServiceProvider.GetService<IFileTransferServiceInternal>().GetFileStream(path);
        }

        #endregion
    }
}
