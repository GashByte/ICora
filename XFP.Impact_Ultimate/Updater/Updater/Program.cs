using System;
using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using System.Web;
using System.IO.Compression;
using XFP.Impact_Ultimate.Updater.Utils;

namespace XFP.Impact_Ultimate.Updater
{
    public class Updater
    {
        public static string path = Environment.CurrentDirectory + "\\Release.zip";
        public static long FileSize;
        public static string DownloadPath = "https://gitee.com/MasterGashByte/updates/releases/download/Release/Release.zip";
        public static int DownloadCount = 0;

        public static void Main()
        {
            Console.WriteLine("正在更新 请不要关闭");
            Console.WriteLine("正在检查库是否存在");
            if (UrlIsExist(DownloadPath))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("检测到库 正在调用下载线程");
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.Green;
                Downloader(DownloadPath);
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("库不存在 请前往群中下载");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }

        private static void Downloader(string url) 
        {
            DownloadCount += 1;
            if (DownloadCount > 5)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("多次下载失败 请前往群中下载");
                Console.ReadKey();
                Environment.Exit(0);
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.WriteLine("-----------------Downloader------------------");
                Console.WriteLine("下载器载入中");
                // HttpDownloader by LoserSkidder
                Console.WriteLine("请求下载中...");
                // 设置参数
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                //发送请求并获取相应回应数据
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                Stopwatch sp = new Stopwatch();
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                Stream responseStream = response.GetResponseStream();
                FileSize = response.ContentLength;
                sp.Start();
                //创建本地文件写入流
                Stream stream = new FileStream(path, FileMode.Create);
                byte[] bArr = new byte[1024];
                int size = responseStream.Read(bArr, 0, (int)bArr.Length);
                while (size > 0)
                {
                    stream.Write(bArr, 0, size);
                    size = responseStream.Read(bArr, 0, (int)bArr.Length);
                }
                stream.Close();
                responseStream.Close();
                sp.Stop();

                Console.WriteLine("下载成功 下载时间：" + sp.ElapsedMilliseconds / 1000.0000 + "秒");
                Console.WriteLine("校验数据完整性中...");

                SizeChecker();
            }
        }

        /// <summary>
        /// 文件大小的校验 by LoserSkidder
        /// </summary>
        private static void SizeChecker()
        {
            Console.WriteLine("-----------------SizeChecker-----------------");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(DownloadPath);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            FileSize = response.ContentLength;
            FileInfo fileInfo = new FileInfo(path);
            Console.WriteLine("下载的数据大小: " + fileInfo.Length + " Kb");
            Console.WriteLine("服务器请求的大小: " + FileSize + " Kb");
            if (fileInfo.Length == FileSize)
            {
                Console.WriteLine("看起来下载的数据很完整");
                Console.WriteLine("载入解压线程...");
                UnZip();
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("喔 数据好像不太完整 正在重新下载呢");
                File.Delete(path);
                Downloader(DownloadPath);
            }
        }

        /// <summary>
        /// 文件解压 by x3zF
        /// </summary>
        private static void UnZip()
        {
            Console.WriteLine("--------------------UnZip--------------------");
            Console.WriteLine("正在解压文件");
            try
            {
                ZipHelper.UnZip(Environment.CurrentDirectory + "\\Release.zip", Environment.CurrentDirectory + "\\");

                Console.WriteLine("解压成功！ 祝您游玩愉快");
                File.Delete(path);
                Environment.Exit(0);
            }
            catch(Exception ex) 
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("出现异常：");
                Console.WriteLine(ex.Message);
                Console.ReadKey();
                Environment.Exit(0);
            }
        }

        private static bool UrlIsExist(string url)
        {
            System.Uri u;
            try
            {
                u = new Uri(url);
            }
            catch { return false; }
            bool isExist = false;
            System.Net.HttpWebRequest? r = System.Net.WebRequest.Create(u) as System.Net.HttpWebRequest;
            r.Method = "HEAD";
            try
            {
                System.Net.HttpWebResponse? s = r.GetResponse() as System.Net.HttpWebResponse;
                if (s.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    isExist = true;
                }
            }
            catch (System.Net.WebException x)
            {
                try
                {
                    isExist = (x.Response as System.Net.HttpWebResponse).StatusCode != System.Net.HttpStatusCode.NotFound;
                }
                catch { isExist = x.Status == System.Net.WebExceptionStatus.Success; }
            }
            return isExist;
        }
    }
}