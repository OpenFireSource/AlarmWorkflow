using System.ComponentModel;

namespace AlarmWorkflow.Tools.AutoUpdater
{
    class InstallOptions
    {
        [DisplayName("Automatisches (de-)installieren des Dienstes")]
        [Description("Deinstalliert den Dienst vor dem Update und installiert ihn erneut nach dem Update.")]
        [DefaultValue(false)]
        public bool AutomaticServiceUnInstall { get; set; }
        [DisplayName("Beenden aller AlarmWorkflow-Prozesse")]
        [Description("Beendet alle laufenden AlarmWorkflow-Prozesse (vorsichtig verwenden!).")]
        [DefaultValue(false)]
        public bool KillAlarmWorkflowProcesses { get; set; }
        [DisplayName("OCR Software installieren")]
        [Description("Nur einmal nötig. Nicht nötig falls OCR-Software bereits auf dem PC installiert (in diese Fall bitte Pfad im Konfigurationseditor angeben!).")]
        [DefaultValue(false)]
        public bool DownloadOcrSoftware { get; set; }
        [DisplayName("Datenbank sichern")]
        [Description("Sichert die Hauptdatenbank (OperationStore) vor dem aktualisieren (empfohlen).")]
        [DefaultValue(true)]
        public bool BackupDatabase { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstallOptions"/> class.
        /// </summary>
        public InstallOptions()
        {
            BackupDatabase = true;
        }
    }
}
