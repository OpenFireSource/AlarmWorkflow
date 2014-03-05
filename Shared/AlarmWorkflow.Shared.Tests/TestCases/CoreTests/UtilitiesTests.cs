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

using System.Collections.Generic;
using AlarmWorkflow.Shared.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AlarmWorkflow.Shared.Tests.Shared.CoreTests
{
    [TestClass()]
    public class UtilitiesTests
    {
        [TestMethod()]
        public void TruncateTest()
        {
            const string text = "0123456789!";
            const int truncateLength = 10;
            const string expectedTruncateLeftEllipsis  = "0123456...";
            const string expectedTruncateRightEllipsis = "...456789!";
            const string expectedTruncateLeftNoEllipsis  = "0123456789";
            const string expectedTruncateRightNoEllipsis = "123456789!";
            
            Assert.AreEqual(expectedTruncateLeftEllipsis, text.Truncate(truncateLength, true, true));
            Assert.AreEqual(expectedTruncateRightEllipsis, text.Truncate(truncateLength, false, true));
            
            Assert.AreEqual(expectedTruncateLeftNoEllipsis, text.Truncate(truncateLength, true, false));
            Assert.AreEqual(expectedTruncateRightNoEllipsis, text.Truncate(truncateLength, false, false));
        }
    }
}