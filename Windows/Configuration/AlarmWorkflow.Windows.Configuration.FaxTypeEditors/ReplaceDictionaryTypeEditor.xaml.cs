using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using AlarmWorkflow.AlarmSource.Fax;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.ConfigurationContracts;

namespace AlarmWorkflow.Windows.Configuration.FaxTypeEditors
{
    /// <summary>
    /// Interaction logic for ReplaceDictionaryTypeEditor.xaml
    /// </summary>
    [Export("ReplaceDictionaryTypeEditor", typeof(ITypeEditor))]
    public partial class ReplaceDictionaryTypeEditor : UserControl, ITypeEditor, INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        /// Gets the ReplaceDictionary-instance for editing.
        /// </summary>
        public ReplaceDictionaryEditWrapper ReplaceDictionary { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ReplaceDictionaryTypeEditor"/> class.
        /// </summary>
        public ReplaceDictionaryTypeEditor()
        {
            InitializeComponent();

            this.DataContext = this;
        }

        #endregion

        #region ITypeEditor Members

        /// <summary>
        /// Gets/sets the value that is edited.
        /// </summary>
        public object Value
        {
            get { return ReplaceDictionary.GetReplaceDictionary(); }
            set
            {
                this.ReplaceDictionary = new ReplaceDictionaryEditWrapper(new ReplaceDictionary((string)value));
                OnPropertyChanged("ReplaceDictionary");
            }
        }

        /// <summary>
        /// Gets the visual element that is editing the value.
        /// </summary>
        public System.Windows.UIElement Visual
        {
            get { return this; }
        }

        void ITypeEditor.Initialize(string editorParameter)
        {
        }

        #endregion

        #region Nested types

        /// <summary>
        /// (Edit wrapper for ReplaceDictionary since DataGrid cannot edit Dictionaries)
        /// </summary>
        public class ReplaceDictionaryEditWrapper : List<FakeKeyValuePair>
        {
            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="ReplaceDictionaryEditWrapper"/> class.
            /// </summary>
            /// <param name="source">The source.</param>
            public ReplaceDictionaryEditWrapper(ReplaceDictionary source)
                : base()
            {
                foreach (var pair in source.Pairs)
                {
                    this.Add(new FakeKeyValuePair() { Key = pair.Key, Value = pair.Value });
                }
            }

            #endregion

            #region Methods

            /// <summary>
            /// Returns a new instance of <see cref="ReplaceDictionary"/> with the contents from this instance.
            /// </summary>
            /// <returns></returns>
            public ReplaceDictionary GetReplaceDictionary()
            {
                ReplaceDictionary dict = new ReplaceDictionary();
                foreach (var fakePair in this)
                {
                    // Skip over null or already existing pairs
                    if (fakePair.Key == null || dict.Pairs.ContainsKey(fakePair.Key))// ||
                    //fakePair.Value == null)
                    {
                        continue;
                    }
                    dict.Pairs.Add(fakePair.Key, fakePair.Value);
                }

                return dict;
            }

            #endregion
        }

        /// <summary>
        /// Simple Key-Value-pair-reference-type to enable editing in WPF's DataGrid (the structure "KeyValuePair" is not allowed for editing).
        /// </summary>
        public class FakeKeyValuePair
        {
            /// <summary>
            /// Key
            /// </summary>
            public string Key { get; set; }
            /// <summary>
            /// Value
            /// </summary>
            public string Value { get; set; }
        }

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raised when the property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            var copy = PropertyChanged;
            if (copy != null)
            {
                copy(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
