//Copyright(c) XFP Group and Contributors. All rights reserved.
//Licensed under the MIT License.   

using HandyControl.Controls;
using System.IO;

namespace XFP.Impact_Ultimate.ICoraException
{
    public class DirectoryNotFound
    {
        /// <summary>
        /// File directory not found
        /// </summary>
        /// <param name="DirectoryName">Directory name that does not exist</param>
        public DirectoryNotFound(string DirectoryName)
        {
            Growl.Error("Throw Exception: Directory Not Found\n Directory Name:" + DirectoryName);
        }

        /// <summary>
        /// File directory not found
        /// Create directory selectively
        /// </summary>
        /// <param name="DirectoryName">Directory name that does not exist</param>
        /// <param name="CreateDirectory">Selectively create the current directory</param>
        public DirectoryNotFound(string DirectoryName, bool CreateDirectory)
        {
            Growl.Warning("Throw Exception: Directory Not Found\n Directory Name:" + DirectoryName);
            if (CreateDirectory == true)
            {
                Directory.CreateDirectory(DirectoryName);
            }
        }
    }
}
