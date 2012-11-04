using System.Diagnostics;

namespace AlarmWorkflow.Shared.Settings
{
    /// <summary>
    /// Display configuration for a single setting.
    /// </summary>
    [DebuggerDisplay("Name = {Name}, Category = {Category}")]
    public class SettingInfo
    {
        #region Properties

        /// <summary>
        /// Gets/sets the name of the setting.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets/sets the category of the setting.
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// Gets/sets the display text of the setting. If there is no display text, the Name will be used.
        /// </summary>
        public string DisplayText { get; set; }
        /// <summary>
        /// Gets/sets the description of the setting.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Gets/sets the order of the setting.
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// Gets/sets the name of the editor of the setting. If this is empty, the default editor will be used.
        /// </summary>
        public string Editor { get; set; }
        /// <summary>
        /// Gets/sets an optional parameter for the editor of the setting.
        /// </summary>
        public string EditorParameter { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingInfo"/> class.
        /// </summary>
        public SettingInfo()
        {

        }

        #endregion
    }
}
