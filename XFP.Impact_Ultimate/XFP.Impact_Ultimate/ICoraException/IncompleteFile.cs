using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XFP.Impact_Ultimate.ICoraException
{
    public class IncompleteFile
    {
        /// <summary>
        /// Incomplete file
        /// </summary>
        /// <param name="FileName">File Name</param>
        public void FileIsNotComplete(string FileName)
        {
            Growl.Error("Throw Exception: File Is Not Found\n File Name:" + FileName);
        }

        /// <summary>
        /// Target File Not Found
        /// </summary>
        /// <param name="TargetFile">Target File</param>
        public void FileNotFound(string TargetFile)
        {
            Growl.Error("Throw Exception: Target File Not Found\n Target File Name:" + TargetFile);
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
            if (CreateTargetFile == true)
                File.Create(TargetFile);
        }
    }
}
