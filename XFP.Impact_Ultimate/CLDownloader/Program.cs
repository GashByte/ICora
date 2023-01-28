using static System.Net.Mime.MediaTypeNames;
using System.Net;
using System.Diagnostics;
using System;

namespace XFP.Impact_Ultimate.CLDownloader
{
    public class CLDownloader
    {
        private static string Impact_UltimatePath = Environment.CurrentDirectory + "\\ICora";
        private static string AkebiPath = Environment.CurrentDirectory + "\\ICora\\CLibrary.dll";

        static void Main()
        {
            if (!Directory.Exists(Impact_UltimatePath))
                Directory.CreateDirectory(Impact_UltimatePath);
            Checker();
        }

        static void Checker()
        {
            Console.WriteLine("-------------------------Checker-------------------------");
            Console.WriteLine("正在检查数据");
            if (File.Exists(AkebiPath))
            {
                Console.WriteLine("检查到了CLibrary的存在 正在校验文件大小");
                FileInfo fileInfo = new FileInfo(AkebiPath);
                long localsize = fileInfo.Length;
                HttpWebRequest request = WebRequest.CreateHttp(
                            "https://gitee.com/MasterGashByte/download/releases/download/CLibrary/CLibrary.dll");
                HttpWebResponse? response = request.GetResponse() as HttpWebResponse;
                Console.WriteLine("您的CL大小: " + localsize);
                Console.WriteLine("服务器的CL大小: " + response.ContentLength);
                SizeChecker(localsize, response.ContentLength);
            }
            else
            {
                Console.WriteLine("没有检查到CLibrary的存在 正在转跳下载线程");
                Downloader();
            }
        }

        static void Downloader()
        {
            Console.WriteLine("-------------------------Downloader-------------------------");
            Console.WriteLine("正在请求文件中");
            HttpWebRequest request = WebRequest.CreateHttp(
                            "https://gitee.com/MasterGashByte/download/releases/download/CLibrary/CLibrary.dll");
            HttpWebResponse? response = request.GetResponse() as HttpWebResponse;
            Stream responseStream = response.GetResponseStream();
            //保存用户头像
            Stream stream = new FileStream(AkebiPath, FileMode.Create);
            byte[] bArr = new byte[1024];
            int size = responseStream.Read(bArr, 0, bArr.Length);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Console.WriteLine("开始下载");
            long totalBytesRead = 0;
            do
            {
                stream.Write(bArr, 0, size);
                size = responseStream.Read(bArr, 0, bArr.Length);
                totalBytesRead += size;
                Console.WriteLine("进度条: " + ((float)totalBytesRead / (float)response.ContentLength) * 100 + "%");
            } while (size > 0);
            stream.Close();
            responseStream.Close();
            sw.Stop();
            FileInfo fileInfo = new FileInfo(AkebiPath);
            long localsize = fileInfo.Length;

            Console.WriteLine("下载成功！ 耗时: " + sw.ElapsedMilliseconds / 1000.0000 + " 秒");
            Console.WriteLine("文件大小: " + localsize);
            Console.WriteLine("正在请求服务器大小: " + response.ContentLength);

            SizeChecker(localsize, response.ContentLength);
        }

        static void SizeChecker(long LocalSize, long ServerSize)
        {
            if (LocalSize != ServerSize)
            {
                Console.WriteLine("这是一个不符合要求的CL正在重新下载");
                File.Delete(AkebiPath);
                Downloader();
            }
            else
            {
                Console.WriteLine("这是一个合格的CL请放心使用");
                Environment.Exit(0);
            }
        }
    }
}
