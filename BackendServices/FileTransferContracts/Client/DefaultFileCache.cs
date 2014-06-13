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
using System.Text;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.BackendService.FileTransferContracts.Client
{
    class DefaultFileCache : IFileCache
    {
        #region Constants

        private const string CacheDirectoryName = "cache";
        private readonly DirectoryInfo _absoluteCacheDirectory;

        #endregion

        #region Constructors

        internal DefaultFileCache()
        {
            _absoluteCacheDirectory = new DirectoryInfo(Path.Combine(Utilities.GetLocalAppDataFolderPath(), CacheDirectoryName));
        }

        #endregion

        #region Methods

        private void EnsureCacheDirectoryExists()
        {
            _absoluteCacheDirectory.Refresh();
            if (!_absoluteCacheDirectory.Exists)
            {
                // Propagate all errors, because this is serious.
                _absoluteCacheDirectory.Create();
            }
        }

        private string GetAbsolutePath(string path)
        {
            Assertions.AssertNotEmpty(path, "path");
            return Path.Combine(_absoluteCacheDirectory.FullName, path);
        }

        private string GetStringHash(string input)
        {
            Assertions.AssertNotEmpty(input, "input");

            return Utilities.ComputeSHA1(Encoding.UTF8.GetBytes(input));
        }

        #endregion

        #region IFileCache Members

        bool IFileCache.IsCached(string path)
        {
            string targetFileName = GetAbsolutePath(GetStringHash(path));
            return File.Exists(targetFileName);
        }

        string IFileCache.GetChecksum(string path)
        {
            using (FileStream stream = File.Open(GetAbsolutePath(GetStringHash(path)), FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Utilities.ComputeMD5(stream);
            }
        }

        Stream IFileCache.OpenFile(string path)
        {
            return File.Open(GetAbsolutePath(GetStringHash(path)), FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        void IFileCache.StoreFile(string path, Stream content)
        {
            Assertions.AssertNotEmpty(path, "path");
            Assertions.AssertNotNull(content, "content");

            string absPath = GetAbsolutePath(GetStringHash(path));

            EnsureCacheDirectoryExists();

            using (FileStream destination = new FileStream(absPath, FileMode.Create, FileAccess.Write))
            {
                content.CopyTo(destination);
            }
        }

        #endregion
    }
}
