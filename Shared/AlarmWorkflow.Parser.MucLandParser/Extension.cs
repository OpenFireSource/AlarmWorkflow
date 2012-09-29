using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.Parser.MucLandParser
{
    [Export("MucLandParser", typeof(IExtension))]
    class Extension : IExtension
    {
        #region IExtension Members

        void IExtension.Initialize(IExtensionHost host)
        {
            host.RegisterParser(new MucLandParser());
        }

        void IExtension.Shutdown()
        {
        }

        #endregion
    }
}
