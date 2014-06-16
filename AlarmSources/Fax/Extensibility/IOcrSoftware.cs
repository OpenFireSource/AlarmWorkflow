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

namespace AlarmWorkflow.AlarmSource.Fax.Extensibility
{
    /// <summary>
    /// Defines mechanisms for a type that represents an OCR-software that is used for parsing incoming faxes.
    /// </summary>
    interface IOcrSoftware
    {
        /// <summary>
        /// Processes the specified image using the OCR software.
        /// </summary>
        /// <param name="options">The processing options that shall be used.</param>
        /// <returns>The processed text contents from the image.</returns>
        string[] ProcessImage(OcrProcessOptions options);
    }
}