using System.ComponentModel;

namespace AlarmWorkflow.Tools.AutoUpdater
{
    class InstallOptions
    {
        [DisplayName("Automatisches (de-)installieren des Dienstes")]
        [Description("Deinstalliert den Dienst vor dem Update und installiert ihn erneut nach dem Update.")]
        public bool AutomaticServiceUnInstall { get; set; }
        [DisplayName("Beenden aller AlarmWorkflow-Prozesse")]
        [Description("Beendet alle laufenden AlarmWorkflow-Prozesse (vorsichtig verwenden!).")]
        public bool KillAlarmWorkflowProcesses { get; set; }
        [DisplayName("Cuneiform installieren")]
        [Description("Nur einmal nötig. Nicht nötig wenn tesseract benutzt wird.")]
        public bool DownloadCuneiform { get; set; }

    }
}
