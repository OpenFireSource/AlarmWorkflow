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

/* 
 * Attention for extending this service!
 * 
 * This service shall never compromise the system the service is running on!
 * For this reason, access to any file outside of the AlarmWorkflow's local directory
 * shall be restricted! The same shall apply to write access - if any.
 * 
 * Also never expose the absolute file paths through an error message.
 * Always work with the relative paths.
 * 
 * This service is not the best idea, security-wise. But sometimes it's just necessary
 * to provide some data to clients.
 * 
 * Like for example, the EMK, allows to register icons for the resources. We may want to
 * provide those icons to clients like the website or the UI, or even an app.
 * 
 * Bottom line: Please be extremely careful with extending this service!
 * 
 */

namespace AlarmWorkflow.BackendService.FileTransfer
{
    class FileTransferServiceInternal : InternalServiceBase, IFileTransferServiceInternal
    {
        #region Methods

        private static void AssertPathIsNotRootedAndExists(string path)
        {
            Assertions.AssertNotEmpty(path, "path");

            if (Path.IsPathRooted(path))
            {
                throw new IOException(Properties.Resources.FilePathIsRootError);
            }

            string localPath = GetLocalPathForRelative(path);
            if (!File.Exists(localPath))
            {
                throw new FileNotFoundException(string.Format(Properties.Resources.FilePathNotFoundError, path));
            }
        }

        private static string GetLocalPathForRelative(string path)
        {
            return Utilities.GetLocalAppDataFolderFileName(path);
        }

        #endregion

        #region IFileTransferServiceInternal Members

        string IFileTransferServiceInternal.GetFileChecksum(string path)
        {
            AssertPathIsNotRootedAndExists(path);

            using (FileStream stream = new FileStream(GetLocalPathForRelative(path), FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Utilities.ComputeMD5(stream);
            }
        }

        Stream IFileTransferServiceInternal.GetFileStream(string path)
        {
            AssertPathIsNotRootedAndExists(path);

            return new FileStream(GetLocalPathForRelative(path), FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        #endregion
    }
}
