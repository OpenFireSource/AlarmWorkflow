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
using AlarmWorkflow.Shared.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AlarmWorkflow.Shared.Tests
{
    /// <summary>
    /// Abstract base class for any test class.
    /// </summary>
    [TestClass()]
    public abstract class TestClassBase
    {
        #region Properties

        /// <summary>
        /// Gets/sets the test context to use for the tests.
        /// </summary>
        public TestContext TestContext { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the full path to a file that lies within the deployed "TestData" directory.
        /// </summary>
        /// <param name="relativePath">The relative file path of the file within the deployed "TestData" directory. Must not be null or empty.</param>
        /// <returns>The full path to a file that lies within the deployed "TestData" directory.</returns>
        protected string GetTestDataFilePath(string relativePath)
        {
            Assertions.AssertNotEmpty(relativePath, "relativePath");
            return Path.Combine(TestContext.DeploymentDirectory, relativePath);
        }

        #endregion
    }
}
