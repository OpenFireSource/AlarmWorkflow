using AlarmWorkflow.Job.MySqlDatabaseJob.Data;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Engine;
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.Job.MySqlDatabaseJob
{
    [Export("MySqlDatabaseJob", typeof(IJob))]
    sealed class MySqlDatabaseJob : IJob
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the DatabaseJob class.
        /// </summary>
        public MySqlDatabaseJob()
        {
        }

        #endregion

        #region IJob Members

        bool IJob.Initialize()
        {
            return AlarmWorkflowEntities.CheckDatabaseReachable();
        }

        void IJob.Execute(IJobContext context, Operation operation)
        {
            if (context.Phase != JobPhase.AfterOperationStored)
            {
                return;
            }

            WriteOperationToDatabase(operation);
        }

        private void WriteOperationToDatabase(Operation operation)
        {
            using (AlarmWorkflowEntities entities = AlarmWorkflowEntities.CreateContext())
            {
                tb_einsatz data = new tb_einsatz();
                data.Einsatznr = operation.OperationNumber;
                data.Einsatzort = operation.Einsatzort.Location;
                data.Einsatzplan = operation.OperationPlan;
                data.Hinweis = operation.Comment;
                data.Kreuzung = operation.Einsatzort.Intersection;
                data.Meldebild = operation.Picture;
                data.Mitteiler = operation.Messenger;
                data.Objekt = operation.Einsatzort.Property;
                data.Ort = operation.Einsatzort.City;
                data.Strasse = operation.Einsatzort.Street + " " + operation.Einsatzort.StreetNumber;
                data.Fahrzeuge = operation.Resources.ToString("{FullName} {RequestedEquipment} | ", null);
                data.Einsatzstichwort = operation.Keywords.EmergencyKeyword;
                data.Stichwort = operation.Keywords.Keyword;
                data.Schleifen = string.Join(";", operation.Loops);
                // TODO: The two times are currently the same. This may change in future.
                //data.ZeitpunktEingang = operation.Timestamp;
                //data.ZeitpunktAlarm = operation.Timestamp;

                entities.tb_einsatz.AddObject(data);
                entities.SaveChanges();
            }
        }

        bool IJob.IsAsync
        {
            get { return false; }
        }

        #endregion

        #region IDisposable Members

        void System.IDisposable.Dispose()
        {

        }

        #endregion
    }
}
