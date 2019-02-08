// This file is part of AlarmWorkflow.
// 
// AlarmWorkflow is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// AlarmWorkflow is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with AlarmWorkflow.  If not, see <http://www.gnu.org/licenses/>.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlarmWorkflow.Backend.ServiceContracts.Communication;
using AlarmWorkflow.BackendService.SettingsContracts;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Settings;
using AlarmWorkflow.Windows.UIContracts.ViewModels;

namespace AlarmWorkflow.Windows.UIWidgets.Operation
{
    public class ViewModel : ViewModelBase, ISettingsServiceCallback
    {
        #region Fields

        private WrappedService<ISettingsService> _service;
        private string _expressionLineOne;
        private string _expressionLineThree;
        private string _expressionLineTwo;
        private readonly int[] _fontSize = new int[3];
        private Shared.Core.Operation _operation;

        #endregion

        #region Properties

        public string LineOne { get; private set; }

        public string LineTwo { get; private set; }

        public string LineThree { get; private set; }

        public int FontSizeOne => _fontSize[0];
        public int FontSizeTwo => _fontSize[1];
        public int FontSizeThree => _fontSize[2];

        #endregion

        #region Overrides of ViewModelBase

        protected override void DisposeCore()
        {
            base.DisposeCore();
            _service.Dispose();
        }

        #endregion

        #region Methods

        private string FormatLine(string expression, Shared.Core.Operation operation)
        {
            if (!string.IsNullOrWhiteSpace(expression))
            {
                try
                {
                    return operation != null ? operation.ToString(expression) : "(n/A)";
                }
                catch (AssertionFailedException)
                {
                    // This exception may occure if the format of the value is broken or other problems with the format exist...
                    return string.Empty;
                }
            }

            return string.Empty;
        }

        private void LoadSettings()
        {
            _expressionLineOne = _service.Instance.GetSetting(SettingKeys.LineOne).GetValue<string>();
            _expressionLineTwo = _service.Instance.GetSetting(SettingKeys.LineTwo).GetValue<string>();
            _expressionLineThree = _service.Instance.GetSetting(SettingKeys.LineThree).GetValue<string>();
            _fontSize[0] = _service.Instance.GetSetting(SettingKeys.SizeOne).GetValue<int>();
            _fontSize[1] = _service.Instance.GetSetting(SettingKeys.SizeTwo).GetValue<int>();
            _fontSize[2] = _service.Instance.GetSetting(SettingKeys.SizeThree).GetValue<int>();
        }


        public void OnOperationChange(Shared.Core.Operation operation)
        {
            _operation = operation;
            SetValues();
        }

        private void SetValues()
        {
            LineOne = FormatLine(_expressionLineOne, _operation);
            LineTwo = FormatLine(_expressionLineTwo, _operation);
            LineThree = FormatLine(_expressionLineThree, _operation);
            OnPropertyChanged(nameof(LineOne));
            OnPropertyChanged(nameof(LineTwo));
            OnPropertyChanged(nameof(LineThree));
        }

        public bool Initialize()
        {
            _service = ServiceFactory.GetCallbackServiceWrapper<ISettingsService>(this);
            LoadSettings();
            return true;
        }


        private void NotifyUi()
        {
            OnPropertyChanged(nameof(FontSizeOne));
            OnPropertyChanged(nameof(FontSizeTwo));
            OnPropertyChanged(nameof(FontSizeThree));
        }

        #endregion

        #region Implementation of ISettingsServiceCallback

        public async void OnSettingChanged(IList<SettingKey> keys)
        {
            if (keys.Any(x => x.Identifier == SettingKeys.Identifier))
            {
                await Task.Run(() => LoadSettings());
                NotifyUi();
                SetValues();
            }
        }

        #endregion
    }
}