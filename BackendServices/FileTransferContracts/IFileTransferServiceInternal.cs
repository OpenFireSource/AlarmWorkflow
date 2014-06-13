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

namespace AlarmWorkflow.BackendService.FileTransferContracts
{
    /// <summary>
    /// Defines methods for a service that provides constrained access to files on the server.
    /// </summary>
    public interface IFileTransferServiceInternal : IInternalService
    {
        /// <summary>
        /// Returns the computed checksum from the requested file.
        /// </summary>
        /// <param name="path">The path of the file to return its checksum. Must not be null.</param>
        /// <returns>The computed checksum (as a hex-formatted string) from the requested file.</returns>
        /// <exception cref="System.IO.FileNotFoundException">The file did not exist.</exception>
        string GetFileChecksum(string path);
        /// <summary>
        /// Returns the file contents as a stream.
        /// </summary>
        /// <param name="path">The path of the file to return. Must not be null.</param>
        /// <returns>The file contents as a stream.</returns>
        /// <exception cref="System.IO.FileNotFoundException">The file did not exist.</exception>
        Stream GetFileStream(string path);
    }
}
