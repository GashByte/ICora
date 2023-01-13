using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace XFP.Impact_Ultimate.Updater.Utils
{
    public class ZipHelper
    {
        /// <summary>  
        /// 功能：解压zip格式的文件。  
        /// </summary>  
        /// <param name="zipFilePath">压缩文件路径</param>  
        /// <param name="unZipDir">解压文件存放路径,为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹</param>  
        /// <returns>解压是否成功</returns>  
        public static bool UnZip(string zipFilePath, string unZipDir)
        {
            try
            {
                if (zipFilePath == string.Empty)
                {
                    throw new Exception("压缩文件不能为空！");
                }
                if (!File.Exists(zipFilePath))
                {
                    throw new FileNotFoundException("压缩文件不存在！");
                }
                //解压文件夹为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹  
                if (unZipDir == string.Empty)
                    unZipDir = zipFilePath.Replace(Path.GetFileName(zipFilePath), Path.GetFileNameWithoutExtension(zipFilePath));
                if (!unZipDir.EndsWith("/"))
                    unZipDir += "/";
                if (!Directory.Exists(unZipDir))
                    Directory.CreateDirectory(unZipDir);
                using (var s = new ZipInputStream(File.OpenRead(zipFilePath)))
                {

                    ZipEntry theEntry;
                    while ((theEntry = s.GetNextEntry()) != null)
                    {
                        string directoryName = Path.GetDirectoryName(theEntry.Name);
                        string fileName = Path.GetFileName(theEntry.Name);
                        if (!string.IsNullOrEmpty(directoryName))
                        {
                            Directory.CreateDirectory(unZipDir + directoryName);
                        }
                        if (directoryName != null && !directoryName.EndsWith("/"))
                        {
                        }
                        if (fileName != String.Empty)
                        {
                            using (FileStream streamWriter = File.Create(unZipDir + theEntry.Name))
                            {

                                int size;
                                byte[] data = new byte[2048];
                                while (true)
                                {
                                    size = s.Read(data, 0, data.Length);
                                    if (size > 0)
                                    {
                                        streamWriter.Write(data, 0, size);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception)
            {

                return false;
            }

        }

        /// <summary>
        /// 压缩所有的文件
        /// </summary>
        /// <param name="filesPath"></param>
        /// <param name="zipFilePath"></param>
        public static void CreateZipFile(string filesPath, string zipFilePath)
        {
            if (!Directory.Exists(filesPath))
            {
                return;
            }
            ZipOutputStream stream = new ZipOutputStream(File.Create(zipFilePath));
            stream.SetLevel(0); // 压缩级别 0-9
            byte[] buffer = new byte[4096]; //缓冲区大小
            string[] filenames = Directory.GetFiles(filesPath, "*.*", SearchOption.AllDirectories);
            foreach (string file in filenames)
            {
                ZipEntry entry = new ZipEntry(file.Replace(filesPath, ""));
                entry.DateTime = DateTime.Now;
                stream.PutNextEntry(entry);
                using (FileStream fs = File.OpenRead(file))
                {
                    int sourceBytes;
                    do
                    {
                        sourceBytes = fs.Read(buffer, 0, buffer.Length);
                        stream.Write(buffer, 0, sourceBytes);
                    } while (sourceBytes > 0);
                }
            }
            stream.Finish();
            stream.Close();
        }
    }
}