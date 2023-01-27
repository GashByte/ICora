//Copyright(c) XFP Group and Contributors. All rights reserved.
//Licensed under the MIT License.   

namespace XFP.ICora.Utils
{
    public class DataProvider
    {
        KeySetter key = new();

        #region definition
        #region public:
        public string Version => "1.3.10";
        public string DataBasePath = Environment.CurrentDirectory + "\\DataBase";
        public string DataLog = Environment.CurrentDirectory + "\\DataBase\\DataLog.log";
        public string TempLog = Environment.CurrentDirectory + "\\DataBase\\TempLog.log";
        public string ErrorLog = Environment.CurrentDirectory + "\\DataBase\\ErrorLog.log";
        public string SettingsData = Environment.CurrentDirectory + "\\DataBase\\Settings.log";
        public ModuleVersion ModeVersion = ModuleVersion.DevelopmentEdition;
        public string HoyolabAccountData = Environment.CurrentDirectory + "\\DataBase\\HoyolabAccountData";
        public string EncryptKey = "ICORAHASX41HBL42KVGO992MLLAS6DJ0";
        #endregion

        #region private:
        private bool IsFristCreateFile = false;
        #endregion
        #endregion

        public DataProvider()
        {
            #region 初始化载入文件
            if (!Directory.Exists(DataBasePath))
                Directory.CreateDirectory(DataBasePath);
            if (!File.Exists(DataLog))
            {
                File.Create(DataLog);
                IsFristCreateFile = true;
                key.sk("Initialized", "False");
            }
            if (!File.Exists(TempLog))
            {
                File.Create(TempLog);
                IsFristCreateFile = true;
                key.sk("Initialized", "False");
            }
            if (!File.Exists(ErrorLog))
            {
                File.Create(ErrorLog);
                IsFristCreateFile = true;
                key.sk("Initialized", "False");
            }
            if (!File.Exists(SettingsData))
            {
                File.Create(SettingsData);
                IsFristCreateFile = true;
                key.sk("Initialized", "False");
            }
            #endregion
            Initialize();
        }

        public void Initialize()
        {
            #region 基础数据写入
            if (IsFristCreateFile == true
                || key.gk("Initialized") == string.Empty
                || key.gk("Initialized") == "False")
            {
                try
                {
                    #region DataLog Basic Log
                    using (StreamWriter sw = new StreamWriter(DataLog, false))
                    {
                        sw.WriteLine("#this file will save some user basic log");
                        sw.WriteLine("#If you deleted it, it means you will lose some data");
                        sw.WriteLine();
                        sw.WriteLine("UserModuleVersion=" + ModuleVersion.BasicEdition.ToString());
                        sw.WriteLine("UserVersion=" + Version);
                        sw.WriteLine("_Initialized=true");
                    }
                    #endregion

                    #region TempLog
                    using (StreamWriter sw = new StreamWriter(TempLog, true))
                    {
                        sw.WriteLine("#this file will save some user temp log");
                        sw.WriteLine("#If you delete it any time If you want to clean up the space");
                        sw.WriteLine();
                    }
                    #endregion

                    #region ErrorLog
                    using (StreamWriter sw = new StreamWriter(ErrorLog, true))
                    {
                        sw.WriteLine("#this file will save some user error log");
                        sw.WriteLine("#It will store all problems If you encounter any problems you can send it to the administrator for repair");
                        sw.WriteLine();
                    }
                    #endregion

                    #region SettingsLog
                    using (StreamWriter sw = new StreamWriter(SettingsData, true))
                    {
                        sw.WriteLine("#this file will save some user settings");
                        sw.WriteLine("#If you delete it, it means you will lose some of your own settings");
                    }
                    #endregion

                    key.sk("Initialized", "True");
                }
                catch { return; }
            }
            #endregion
        }
    }
}
