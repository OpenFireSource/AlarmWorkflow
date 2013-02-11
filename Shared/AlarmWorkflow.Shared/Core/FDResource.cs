using System.Diagnostics;

namespace AlarmWorkflow.Shared.Core
{
    /// <summary>
    /// Specifies the details of a single unit (vehicle/resource/etc.) of a fire department.
    /// </summary>
    [DebuggerDisplay("Identifier = {Identifier}, DisplayName = {DisplayName}")]
    public sealed class FDResource
    {
        #region Properties

        /// <summary>
        /// Gets/sets the unique identifier. This usually matches the name
        /// from the alarm source for this resource and is contained within <see cref="P:OperationResource.FullName"/>.
        /// </summary>
        public string Identifier { get; set; }
        /// <summary>
        /// Gets/sets the display friendly name for this unit.
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// Gets/sets the path of the image representing this resource, to be used in UI components.
        /// See documentation for further information.
        /// </summary>
        /// <remarks>This path shall be relative to the ProgramData-directory and not be absolute.</remarks>
        public string ImagePath { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FDResource"/> struct.
        /// </summary>
        public FDResource()
        {
            Identifier = string.Empty;
            DisplayName = string.Empty;
            ImagePath = string.Empty;
        }

        #endregion
    }
}
