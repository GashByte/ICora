using HandyControl.Controls;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text.RegularExpressions;
using Windows.ApplicationModel.VoiceCommands;
using XFP.Impact_Ultimate.Utlis.Model.Settings;

namespace XFP.Impact_Ultimate.Utlis.Log
{
    public class SetterWriter : UserSettings
    {
        #region initialization
        DataProvider data = new();
        #endregion

        public void read()
        {
            string[] lines = File.ReadAllLines(data.SettingsData);

            for (int i = 1; i < lines.Length; i++)
            { 
                string line = lines[i];

                if (line != string.Empty)
                {
                    Match match = Regex.Match(line, "(?<option>\\w+)=(?<value>.*)");
                    if (match.Success)
                    {
                        string Options = match.Groups["option"].Value;
                        string Value = match.Groups["value"].Value;

                        MessageBox.Show(Options + " " + Value);
                    }
                }
                else
                {
                    MessageBox.Show("2");
                }
            }
        }

        private void Set(string Options, string Value)
        {
            Type OptionType = typeof(UserSettings);
            
            var fields = OptionType.GetFields();

            foreach (var field in fields) 
            {
                var dataType = field.FieldType;


            }
        }
    }
}
