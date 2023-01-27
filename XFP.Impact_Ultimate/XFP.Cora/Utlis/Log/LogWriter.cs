//Copyright(c) XFP Group and Contributors. All rights reserved.
//Licensed under the MIT License.   

namespace XFP.ICora.Utils.Log
{
    public class LogWriter
    {
        DataProvider data = new();

        public void ErrorLog(string ErrorMessage, int returnCode, string Solution)
        {
            var LogPath = data.ErrorLog;
            try
            {
                using (StreamWriter sw = new StreamWriter(LogPath, true))
                {
                    sw.WriteLine();
                    sw.WriteLine("##################### Error Log #####################");
                    sw.WriteLine("Error Message: ");
                    sw.WriteLine(ErrorMessage);
                    sw.WriteLine("Return Code:");
                    sw.WriteLine(returnCode);
                    sw.WriteLine("Time");
                    DateTime dt = DateTime.Now;
                    sw.WriteLine(dt.ToString());
                    sw.WriteLine("Solution:");
                    sw.WriteLine(Solution);
                    sw.WriteLine("##################### Error Log #####################");
                }
            }
            catch (Exception ex)
            {
                ErrorLog(ex.Message, -1, "这是未知的问题");
            }
        }

        public void ErrorLog(string ErrorMessage, int returnCode)
        {
            var LogPath = data.ErrorLog;
            try
            {
                using (StreamWriter sw = new StreamWriter(LogPath, true))
                {
                    sw.WriteLine();
                    sw.WriteLine("##################### Error Log #####################");
                    sw.WriteLine("Error Message: ");
                    sw.WriteLine(ErrorMessage);
                    sw.WriteLine("Return Code:");
                    sw.WriteLine(returnCode);
                    sw.WriteLine("Time");
                    DateTime dt = DateTime.Now;
                    sw.WriteLine(dt.ToString());
                    sw.WriteLine("##################### Error Log #####################");
                }
            }
            catch (Exception ex)
            {
                ErrorLog(ex.Message, -1, "这是未知的问题");
            }
        }

        public void TempLog(string TempMessage)
        {
            var LogPath = data.TempLog;
            try
            {
                using (StreamWriter sw = new StreamWriter(LogPath, true))
                {
                    sw.WriteLine();
                    sw.WriteLine(TempMessage);
                }
            }
            catch (Exception ex)
            {
                ErrorLog(ex.Message, -1, "这是未知的问题");
            }
        }

        public void DataLog(string DataMessage)
        {
            var LogPath = data.DataLog;
            try
            {
                using (StreamWriter sw = new StreamWriter(LogPath, false))
                {
                    sw.WriteLine(DataMessage);
                }
            }
            catch (Exception ex)
            {
                ErrorLog(ex.Message, -1, "这是未知的问题");
            }
        }
    }
}
