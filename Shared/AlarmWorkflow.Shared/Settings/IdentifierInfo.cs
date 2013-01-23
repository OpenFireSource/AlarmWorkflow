using System.Collections.Generic;
using System.Diagnostics;

namespace AlarmWorkflow.Shared.Settings
{
    /// <summary>
    /// Display configuration for an identifier section.
    /// </summary>
    [DebuggerDisplay("Name = {Name}, Settings count = {Settings.Count}")]
    public class IdentifierInfo
    {
        #region Properties

        /// <summary>
        /// Gets/sets the name of the identifier section.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets/sets the display text of the identifier section. If there is none, the Name will be used.
        /// </summary>
        public string DisplayText { get; set; }
        /// <summary>
        /// Gets/sets the description of the identifier section.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Gets/sets the order of the identifier section.
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// Gets/sets the identifier of the parent section.
        /// Use null for no parent section.
        /// </summary>
        public string Parent { get; set; }

        /// <summary>
        /// Gets/sets the settings that are contained in this identifier section.
        /// </summary>
        public List<SettingInfo> Settings { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentifierInfo"/> class.
        /// </summary>
        public IdentifierInfo()
        {
            Settings = new List<SettingInfo>();
        }

        #endregion
    }
}
