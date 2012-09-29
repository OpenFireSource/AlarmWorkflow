using System.Collections.Generic;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Shared.Extensibility
{
    /// <summary>
    /// Defines a parser which parses an incoming alarmfax.
    /// </summary>
    public interface IParser : IExtensionObject
    {
        /// <summary>
        /// This Methode is parsing a ocr text file and fill an einsatz object.
        /// </summary>
        /// <param name="replaceList">A list containing dictionary entries that define how one word should be replaced with another.</param>
        /// <param name="file">Full path to the ocr file.</param>
        /// <returns>A filled einsatz object.</returns>
        Operation Parse(IList<ReplaceString> replaceList, string file);
    }
}
