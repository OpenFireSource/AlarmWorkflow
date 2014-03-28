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
using System.IO;
using AlarmWorkflow.Backend.ServiceContracts.Communication;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.BackendService.FileTransferContracts.Client
{
    /// <summary>
    /// Provides functionality to access the <see cref="IFileTransferService"/> and a fast cache to minimize network usage.
    /// See documentation for usage and further information.
    /// </summary>
    /// <remarks>This class uses the <see cref="ServiceFactory"/> to create a connection to the configured server.
    /// It is only intended to be used from within a connected client. Never use this class when you are within the service.</remarks>
    public sealed class FileTransferServiceClient : DisposableObject
    {
        #region Fields

        private IFileCache _fileCache;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FileTransferServiceClient"/> class.
        /// </summary>
        public FileTransferServiceClient()
        {
            _fileCache = new DefaultFileCache();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Overridden to clean up all used resources.
        /// </summary>
        protected override void DisposeCore()
        {

        }

        /// <summary>
        /// Returns a stream to the requested file. Don't forget to close the file once you're finished!
        /// See documentation for further information.
        /// </summary>
        /// <remarks>If the file does not exist in the local cache, it is fetched and cached.
        /// If the file does exist, its checksum is compared with the one from the server-side file
        /// and may be cached again and returned.</remarks>
        /// <param name="path">The path to the file to return. Must be a relative path and not be null or empty.</param>
        /// <returns>A <see cref="Stream"/> containing the file contents.</returns>
        /// <exception cref="System.IO.IOException">The file was not found.
        /// -or- An error occurred retrieving the file from the local cache directory.
        /// -or- An error occurred retrieving the file from the server.</exception>
        public Stream GetFileFromPath(string path)
        {
            AssertNotDisposed();

            Assertions.AssertNotEmpty(path, "path");

            try
            {
                using (var service = ServiceFactory.GetServiceWrapper<IFileTransferService>())
                {
                    if (!_fileCache.IsCached(path))
                    {
                        GetFileAndStoreInCache(path, service.Instance);
                    }
                    else
                    {
                        string sourceChecksum = service.Instance.GetFileChecksum(path);
                        string cacheFileChecksum = _fileCache.GetChecksum(path);
                        if (!string.Equals(sourceChecksum, cacheFileChecksum))
                        {
                            GetFileAndStoreInCache(path, service.Instance);
                        }
                    }

                    return _fileCache.OpenFile(path);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(this, ex);
                throw new IOException(string.Format(Properties.Resources.GetFileFromPathRetrievalError, path), ex);
            }
        }

        private void GetFileAndStoreInCache(string path, IFileTransferService service)
        {
            Stream content = service.GetFileStream(path);
            _fileCache.StoreFile(path, content);
        }

        #endregion

    }
}
