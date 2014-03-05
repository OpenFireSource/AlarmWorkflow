using AlarmWorkflow.Shared.Settings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AlarmWorkflow.Shared.Tests.Shared.SettingsTests
{
    [TestClass()]
    public class StringSettingConvertibleToolsTests : TestClassBase
    {
        #region Constants

        private const string MockValue = "Hello world!";

        #endregion

        #region Methods

        [TestMethod()]
        public void ConvertBackTest()
        {
            MockConvertible mock = new MockConvertible() { Value = MockValue };

            string value = null;
            bool result = StringSettingConvertibleTools.ConvertBack(mock, out value);

            Assert.IsTrue(result);
            Assert.AreEqual(mock.Value, value);
        }

        [TestMethod()]
        public void ConvertFromSettingTest()
        {
            MockConvertible mock = StringSettingConvertibleTools.ConvertFromSetting<MockConvertible>(MockValue);
            Assert.AreEqual(MockValue, mock.Value);

            mock = (MockConvertible)StringSettingConvertibleTools.ConvertFromSetting(typeof(MockConvertible), MockValue);
            Assert.AreEqual(MockValue, mock.Value);
        }

        class MockConvertible : IStringSettingConvertible
        {
            #region Property

            public string Value { get; set; }

            #endregion

            #region IStringSettingConvertible Members

            void IStringSettingConvertible.Convert(string settingValue)
            {
                Value = settingValue;
            }

            string IStringSettingConvertible.ConvertBack()
            {
                return Value;
            }

            #endregion
        }

        #endregion

    }
}
