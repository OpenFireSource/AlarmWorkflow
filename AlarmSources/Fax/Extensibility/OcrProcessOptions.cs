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
    /// Defines the options that shall be used by the OCR-software when processing a new image.
    /// </summary>
    sealed class OcrProcessOptions
    {
        /// <summary>
        /// Gets the manually specified path to the OCR-software to use. If this is null, then the default path shall be used.
        /// </summary>
        public string SoftwarePath { get; internal set; }
        /// <summary>
        /// Gets the path to the image file to process.
        /// </summary>
        public string ImagePath { get; internal set; }
        /// <summary>
        /// Gets the file name where the analyzed file shall be stored.
        /// </summary>
        public string AnalyzedFileDestinationPath { get; internal set; }
    }
}