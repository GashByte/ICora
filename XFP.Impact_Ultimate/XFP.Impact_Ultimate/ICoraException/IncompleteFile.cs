//Copyright(c) XFP Group and Contributors. All rights reserved.
//Licensed under the MIT License.   

using HandyControl.Controls;
using System.IO;
using XFP.Impact_Ultimate.Utils.Log;

namespace XFP.Impact_Ultimate.ICoraException
{
    public class IncompleteFile
    {
        LogWriter log = new();

        /// <summary>
        /// Incomplete file
        /// </summary>
        /// <param name="FileName">File Name</param>
        public void FileIsNotComplete(string FileName)
        {
            Growl.Error("Throw Exception: File Is Not Found\n File Name:" + FileName);
            log.ErrorLog("Throw Exception: File Is Not Found\n File Name:" + FileName, -10);
        }

        /// <summary>
        /// Target File Not Found
        /// </summary>
        /// <param name="TargetFile">Target File</param>
        public void FileNotFound(string TargetFile)
        {
            Growl.Error("Throw Exception: Target File Not Found\n Target File Name:" + TargetFile);
            log.ErrorLog("Throw Exception: Target File Not Found\n Target File Name:" + TargetFile, -10);
        }

        /// <summary>
        /// Target File Not Found
        /// Create directory selectively
        /// </summary>
        /// <param name="TargetFile">Target File</param>
        /// <param name="CreateTargetFile">Selectively create the current directory</param>
        public void FileNotFound(string TargetFile, bool CreateTargetFile)
        {
            Growl.Warning("Throw Exception: Target File Not Found\n Target File Name:" + TargetFile);
            log.ErrorLog("Throw Exception: Target File Not Found\n Target File Name:" + TargetFile, -10);
            if (CreateTargetFile == true)
            {
                File.Create(TargetFile);
                log.TempLog("Create File" + TargetFile);
            }
        }
    }
}
