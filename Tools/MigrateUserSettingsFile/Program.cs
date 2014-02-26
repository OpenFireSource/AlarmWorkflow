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

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using AlarmWorkflow.Backend.ServiceContracts.Data;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Tools.MigrateUserSettingsFile.Data;
using AlarmWorkflow.Tools.MigrateUserSettingsFile.Properties;

namespace AlarmWorkflow.Tools.MigrateUserSettingsFile
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.Instance.Initialize("MigrateUserSettingsFile");

            // Print welcome information :-)
            Console.WriteLine("********************************************************");
            Console.WriteLine("*                                                      *");
            Console.WriteLine("*   AlarmWorkflow user.settings migration tool         *");
            Console.WriteLine("*                                                      *");
            Console.WriteLine("********************************************************");
            Console.WriteLine();
            Console.WriteLine(Resources.WelcomeMessage);
            Console.Write("> ");

            string inputPath = Console.ReadLine();
            if (inputPath == string.Empty)
            {
                inputPath = Utilities.GetLocalAppDataFolderFileName("user.settings");
            }

            if (File.Exists(inputPath))
            {
                XDocument doc = XDocument.Load(inputPath);
                if (doc.IsXmlValid(Resources.UserSettingsSchema))
                {
                    TryImportUserSettings(doc);
                }
                else
                {
                    Logger.Instance.LogFormat(LogType.Error, null, Resources.UserSettingsSchemaValidationFailure);
                }
            }
            else
            {
                Logger.Instance.LogFormat(LogType.Error, null, Resources.FileDoesNotExist);
            }

            Thread.Sleep(2000);
        }

        private static void TryImportUserSettings(XDocument doc)
        {
            try
            {
                ImportUserSettings(doc);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogFormat(LogType.Error, null, Resources.OverallImportError);
                Logger.Instance.LogException(null, ex);
            }
            finally
            {
                Console.WriteLine(Resources.ProcessFinished);
            }
        }

        private static void ImportUserSettings(XDocument doc)
        {
            using (MigrationSettingsEntities entities = EntityFrameworkHelper.CreateContext<MigrationSettingsEntities>("Data.SettingsEntities"))
            {
                foreach (XElement section in doc.Root.Elements("Section"))
                {
                    string identifier = section.Attribute("Identifier").Value;
                    foreach (XElement setting in section.Elements("UserSetting"))
                    {
                        try
                        {
                            string name = setting.Attribute("Name").Value;

                            bool isNew = true;
                            UserSettingData data = entities.UserSettings.FirstOrDefault(item => item.Identifier == identifier && item.Name == name);
                            if (data == null)
                            {
                                data = new UserSettingData();
                            }
                            else
                            {
                                isNew = false;
                                Logger.Instance.LogFormat(LogType.Warning, null, Resources.OverwritingExistingItem, identifier, name);
                            }

                            data.Identifier = identifier;
                            data.Name = name;
                            data.Value = setting.Value;

                            XAttribute isNull = setting.Attribute("IsNull");
                            if (isNull != null && isNull.Value.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase))
                            {
                                data.Value = null;
                            }

                            if (isNew)
                            {
                                entities.UserSettings.AddObject(data);
                            }

                            Logger.Instance.LogFormat(LogType.Info, null, Resources.SettingWritten, data.Identifier, data.Name);
                        }
                        catch (Exception ex)
                        {
                            Logger.Instance.LogFormat(LogType.Error, null, Resources.SettingWriteError);
                            Logger.Instance.LogException(null, ex);
                        }
                    }
                }

                entities.SaveChanges();
            }
        }
    }
}
