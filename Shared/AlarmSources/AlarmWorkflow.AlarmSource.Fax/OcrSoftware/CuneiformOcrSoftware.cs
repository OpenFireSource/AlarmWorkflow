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
using AlarmWorkflow.AlarmSource.Fax.Extensibility;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.AlarmSource.Fax.OcrSoftware
{
    [Obsolete("Cuneiform is obsolete as of v0.9.5 and will be removed in the next version. Please switch to tesseract!")]
    [Export("Cuneiform", typeof(IOcrSoftware))]
    [Information(DisplayName = "ExportCuneiformOcrDisplayName", Description = "ExportCuneiformOcrDescription")]
    sealed class CuneiformOcrSoftware : IOcrSoftware
    {
        #region IOcrSoftware Members

        string[] IOcrSoftware.ProcessImage(OcrProcessOptions options)
        {
            return new string[0];
        }

        #endregion
    }
}