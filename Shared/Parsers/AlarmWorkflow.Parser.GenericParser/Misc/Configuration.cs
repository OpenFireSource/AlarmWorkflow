using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.Parser.GenericParser.Misc
{
    class Configuration
    {
        internal string ControlFile { get; private set; }

        internal Configuration()
        {
            ControlFile = SettingsManager.Instance.GetSetting("GenericParser", "ControlFile").GetString();
        }
    }
}
