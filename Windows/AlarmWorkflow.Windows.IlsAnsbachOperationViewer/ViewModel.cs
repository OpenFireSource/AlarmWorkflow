using System.Linq;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.UI.ViewModels;

namespace AlarmWorkflow.Windows.IlsAnsbachOperationViewer
{
    class ViewModel : ViewModelBase
    {
        #region Fields

        private Operation _operation;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the operation.
        /// </summary>
        public Operation Operation
        {
            get { return _operation; }
            set
            {
                _operation = value;

                // Set operation itself
                OnPropertyChanged("Operation");

                // Update all other properties
                UpdateProperties();
            }
        }

        /// <summary>
        /// Gets the value from the "Absender" custom data field.
        /// </summary>
        public string Absender
        {
            get { return GetOperationCustomData<string>("Absender", null); }
        }

        /// <summary>
        /// Gets the value from the "Termin" custom data field.
        /// </summary>
        public string Termin
        {
            get { return GetOperationCustomData<string>("Termin", null); }
        }

        /// <summary>
        /// Gets the value from the "Einsatzort Plannummer" custom data field.
        /// </summary>
        public string EinsatzortPlannummer
        {
            get { return GetOperationCustomData<string>("Einsatzort Plannummer", null); }
        }

        /// <summary>
        /// Gets the value from the "Einsatzort Station" custom data field.
        /// </summary>
        public string EinsatzortStation
        {
            get { return GetOperationCustomData<string>("Einsatzort Station", null); }
        }

        /// <summary>
        /// Gets the value from the "Zielort Hausnummer" custom data field.
        /// </summary>
        public string ZielortHausnummer
        {
            get { return GetOperationCustomData<string>("Zielort Hausnummer", null); }
        }

        /// <summary>
        /// Gets the value from the "Zielort Straße" custom data field.
        /// </summary>
        public string ZielortStrasse
        {
            get { return GetOperationCustomData<string>("Zielort Straße", null); }
        }

        /// <summary>
        /// Gets the value from the "Zielort PLZ" custom data field.
        /// </summary>
        public string ZielortPLZ
        {
            get { return GetOperationCustomData<string>("Zielort PLZ", null); }
        }

        /// <summary>
        /// Gets the value from the "Zielort Ort" custom data field.
        /// </summary>
        public string ZielortOrt
        {
            get { return GetOperationCustomData<string>("Zielort Ort", null); }
        }

        /// <summary>
        /// Gets the value from the "Zielort Objekt" custom data field.
        /// </summary>
        public string ZielortObjekt
        {
            get { return GetOperationCustomData<string>("Zielort Objekt", null); }
        }

        /// <summary>
        /// Gets the value from the "Zielort Station" custom data field.
        /// </summary>
        public string ZielortStation
        {
            get { return GetOperationCustomData<string>("Zielort Station", null); }
        }

        /// <summary>
        /// Gets the value from the "Stichwort B" custom data field.
        /// </summary>
        public string StichwortB
        {
            get { return GetOperationCustomData<string>("Stichwort B", null); }
        }

        /// <summary>
        /// Gets the value from the "Stichwort R" custom data field.
        /// </summary>
        public string StichwortR
        {
            get { return GetOperationCustomData<string>("Stichwort R", null); }
        }

        /// <summary>
        /// Gets the value from the "Stichwort S" custom data field.
        /// </summary>
        public string StichwortS
        {
            get { return GetOperationCustomData<string>("Stichwort S", null); }
        }

        /// <summary>
        /// Gets the value from the "Stichwort T" custom data field.
        /// </summary>
        public string StichwortT
        {
            get { return GetOperationCustomData<string>("Stichwort T", null); }
        }

        /// <summary>
        /// Gets the value from the "Priorität" custom data field.
        /// </summary>
        public string Priorität
        {
            get { return GetOperationCustomData<string>("Priorität", null); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModel"/> class.
        /// </summary>
        public ViewModel()
        {

        }

        #endregion

        #region Methods

        private T GetOperationCustomData<T>(string key, T defaultValue)
        {
            if (_operation != null && _operation.CustomData != null && _operation.CustomData.ContainsKey(key))
            {
                return (T)_operation.CustomData[key];
            }

            return defaultValue;
        }

        private void UpdateProperties()
        {
            foreach (var pi in this.GetType().GetProperties().Where(p => p.Name != "Operation"))
            {
                OnPropertyChanged(pi.Name);
            }
        }

        #endregion

        #region Nested types

        class ResourceViewModel : ViewModelBase
        {

        }

        #endregion
    }
}
