//Copyright(c) XFP Group and Contributors. All rights reserved.
//Licensed under the MIT License.

using HandyControl.Controls;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using XFP.Impact_Ultimate.ICoraException;
using XFP.Impact_Ultimate.Utlis;
using XFP.Impact_Ultimate.Utlis.Log;

namespace XFP.Impact_Ultimate.Controls.Game.Utils
{
    public class GameConverter
    {
        KeySetter key = new();
        LogWriter log = new();

        #region 库文件
        private string[] GlobalFiles = new string[]
        {
            "GenshinImpact.exe",
            "HoYoKProtect.sys",
            "mhypbase.dll",
            "pkg_version",
            "UnityPlayer.dll",
            "GenshinImpact_Data/app.info",
            "GenshinImpact_Data/blueReporter.exe",
            "GenshinImpact_Data/globalgamemanagers",
            "GenshinImpact_Data/globalgamemanagers.assets",
            "GenshinImpact_Data/globalgamemanagers.assets.resS",
            "GenshinImpact_Data/upload_crash.exe",
            "GenshinImpact_Data/Native/UserAssembly.dll",
            "GenshinImpact_Data/Native/UserAssembly.exp",
            "GenshinImpact_Data/Native/UserAssembly.lib",
            "GenshinImpact_Data/Plugins/Astrolabe.dll",
            "GenshinImpact_Data/Plugins/chrome_elf.dll",
            "GenshinImpact_Data/Plugins/crashreport.exe",
            "GenshinImpact_Data/Plugins/cri_mana_vpx.dll",
            "GenshinImpact_Data/Plugins/cri_vip_unity_pc.dll",
            "GenshinImpact_Data/Plugins/cri_ware_unity.dll",
            "GenshinImpact_Data/Plugins/d3dcompiler_47.dll",
            "GenshinImpact_Data/Plugins/hdiffz.dll",
            "GenshinImpact_Data/Plugins/hpatchz.dll",
            "GenshinImpact_Data/Plugins/kcp.dll",
            "GenshinImpact_Data/Plugins/libEGL.dll",
            "GenshinImpact_Data/Plugins/libGLESv2.dll",
            "GenshinImpact_Data/Plugins/libUbiCustomEvent.dll",
            "GenshinImpact_Data/Plugins/mailbox.dll",
            "GenshinImpact_Data/Plugins/MiHoYoMTRSDK.dll",
            "GenshinImpact_Data/Plugins/mihoyonet.dll",
            "GenshinImpact_Data/Plugins/MiHoYoSDKUploader.dll",
            "GenshinImpact_Data/Plugins/Mmoron.dll",
            "GenshinImpact_Data/Plugins/MTBenchmark_Windows.dll",
            "GenshinImpact_Data/Plugins/NamedPipeClient.dll",
            "GenshinImpact_Data/Plugins/Rewired_DirectInput.dll",
            "GenshinImpact_Data/Plugins/Telemetry.dll",
            "GenshinImpact_Data/Plugins/UnityNativeChromaSDK.dll",
            "GenshinImpact_Data/Plugins/UnityNativeChromaSDK3.dll",
            "GenshinImpact_Data/Plugins/vk_swiftshader.dll",
            "GenshinImpact_Data/Plugins/vulkan-1.dll",
            "GenshinImpact_Data/Plugins/xlua.dll",
            "GenshinImpact_Data/Plugins/ZFEmbedWeb.dll",
            "GenshinImpact_Data/Plugins/ZFGameBrowser.exe",
            "GenshinImpact_Data/Plugins/ZFProxyWeb.dll",
            "GenshinImpact_Data/Plugins/zf_cef.dll",
            "GenshinImpact_Data/StreamingAssets/20527480.blk",
            "GenshinImpact_Data/Managed/Metadata/global-metadata.dat",
            "GenshinImpact_Data/SDKCaches/telemetry/Preferences",
            "GenshinImpact_Data/SDKCaches/telemetry/TelemetryServiceplat_explog_sdk_v2.db",
            "GenshinImpact_Data/Native/Data/Metadata/global-metadata.dat",
        };

        private string[] ChineseFiles = new string[]
        {
            "HoYoKProtect.sys",
            "mhypbase.dll",
            "pkg_version",
            "UnityPlayer.dll",
            "YuanShen.exe",
            "YuanShen_Data/app.info",
            "YuanShen_Data/blueReporter.exe",
            "YuanShen_Data/globalgamemanagers",
            "YuanShen_Data/globalgamemanagers.assets",
            "YuanShen_Data/globalgamemanagers.assets.resS",
            "YuanShen_Data/upload_crash.exe",
            "YuanShen_Data/Native/UserAssembly.dll",
            "YuanShen_Data/Native/UserAssembly.exp",
            "YuanShen_Data/Native/UserAssembly.lib",
            "YuanShen_Data/Plugins/Astrolabe.dll",
            "YuanShen_Data/Plugins/chrome_elf.dll",
            "YuanShen_Data/Plugins/crashreport.exe",
            "YuanShen_Data/Plugins/cri_mana_vpx.dll",
            "YuanShen_Data/Plugins/cri_vip_unity_pc.dll",
            "YuanShen_Data/Plugins/cri_ware_unity.dll",
            "YuanShen_Data/Plugins/d3dcompiler_47.dll",
            "YuanShen_Data/Plugins/hdiffz.dll",
            "YuanShen_Data/Plugins/hpatchz.dll",
            "YuanShen_Data/Plugins/kcp.dll",
            "YuanShen_Data/Plugins/libEGL.dll",
            "YuanShen_Data/Plugins/libGLESv2.dll",
            "YuanShen_Data/Plugins/libUbiCustomEvent.dll",
            "YuanShen_Data/Plugins/mailbox.dll",
            "YuanShen_Data/Plugins/MiHoYoMTRSDK.dll",
            "YuanShen_Data/Plugins/mihoyonet.dll",
            "YuanShen_Data/Plugins/MiHoYoSDKUploader.dll",
            "YuanShen_Data/Plugins/Mmoron.dll",
            "YuanShen_Data/Plugins/MTBenchmark_Windows.dll",
            "YuanShen_Data/Plugins/NamedPipeClient.dll",
            "YuanShen_Data/Plugins/Rewired_DirectInput.dll",
            "YuanShen_Data/Plugins/Telemetry.dll",
            "YuanShen_Data/Plugins/UnityNativeChromaSDK.dll",
            "YuanShen_Data/Plugins/UnityNativeChromaSDK3.dll",
            "YuanShen_Data/Plugins/vk_swiftshader.dll",
            "YuanShen_Data/Plugins/vulkan-1.dll",
            "YuanShen_Data/Plugins/xlua.dll",
            "YuanShen_Data/Plugins/ZFEmbedWeb.dll",
            "YuanShen_Data/Plugins/ZFGameBrowser.exe",
            "YuanShen_Data/Plugins/ZFProxyWeb.dll",
            "YuanShen_Data/Plugins/zf_cef.dll",
            "YuanShen_Data/StreamingAssets/20527480.blk",
            "YuanShen_Data/Managed/Metadata/global-metadata.dat",
            "YuanShen_Data/SDKCaches/telemetry/Preferences",
            "YuanShen_Data/SDKCaches/telemetry/TelemetryServiceplat_explog_sdk_v2.db",
            "YuanShen_Data/Native/Data/Metadata/global-metadata.dat",
        };
        #endregion

        private string GenshinServiceDir = Environment.CurrentDirectory + "\\GenshinService";

        public void Converter()
        {
            DirectoryInfo info = new DirectoryInfo(key.gk("Genshin Impact Path"));
            string GenshinDir = info.Parent.FullName;

            if (!(!File.Exists(GenshinDir + "\\YuanShen.exe")
                || !File.Exists(GenshinDir + "\\GenshinImpact.exe")))
            {
                new DirectoryNotFound("缺少原神文件夹中的核心文件");
            }

            if (File.Exists(GenshinDir + "\\YuanShen.exe"))
            {
                CronvertFilesAsync(IService.Global);
            }
            else if (File.Exists(GenshinDir + "\\GenshinImpact.exe"))
            {
                CronvertFilesAsync(IService.Chinese);
            }
            else
            {
                new DirectoryNotFound("缺少原神文件夹中的核心文件");
            }
        }

        /// <summary>
        /// 异步覆盖当前服务器
        /// </summary>
        /// <param name="Service"></param>
        /// <returns></returns>
        public async Task CronvertFilesAsync(IService Service)
        {
            string GlobalFilePath = GenshinServiceDir + "\\Replace_file_v3.3.0.zip";
            string CNFilePath = GenshinServiceDir + "\\Initial_file_v3.3.0.zip";

            // decide the Serivce arguments
            string GenshinMain = string.Empty;
            string[] GenshinService = new string[] { string.Empty };
            if (Service == IService.Chinese)
            {
                GenshinService = GlobalFiles;
            }
            else
            {
                GenshinService = ChineseFiles;
            }
            // Convet File By LoserSkidder
            // Create the BackUp Directory

            DirectoryInfo info = new DirectoryInfo(key.gk("Genshin Impact Path"));
            string GenshinDir = info.Parent.FullName;

            try
            {
                // Review the selections
                string FilePath = GenshinService == ChineseFiles ? CNFilePath : GlobalFilePath;
                if (FilePath == string.Empty)
                {
                    // Exception
                    new UnKnownChoose("Un Knwon User Choose");
                    return;
                }

                // Check for duplicate files
                StringBuilder stringbuilder = new StringBuilder();
                foreach (var str in GenshinService)
                {
                    stringbuilder.Append(str.ToString() + "\n");
                }

                using (ZipFile zip = new ZipFile(FilePath))
                {
                    string list = string.Empty;
                    foreach (ZipEntry entry in zip)
                    {
                        list += entry.Name + "\n";
                        // check the File is Exists
                        if (!stringbuilder.ToString().Contains(entry.Name))
                        {
                            // Exception
                            new IncompleteFile().FileIsNotComplete(entry.Name);
                            return;
                        }

                        // Delete duplicate files

                        // Check the list is the Directory or not
                        string resultName = GenshinDir + "\\" + entry.Name;
                        // if list is not Directory

                        // If this file or directory is the content : return
                        if (!(resultName == GenshinDir + "\\YuanShen_Data/"
                            || resultName == GenshinDir + "\\GenShinImpact_Data/"))
                        {
                            // Delete the File
                            FileInfo Dinfo = new FileInfo(resultName);
                            if (Dinfo.Attributes != FileAttributes.Directory)
                            {
                                File.Delete(resultName);
                            }
                        }
                    }

                    // Convert the Genshin Files
                    try
                    {
                        // Modify the core directory name
                        if (Service == IService.Global)
                        {
                            Directory.Move(GenshinDir + "\\YuanShen_Data", GenshinDir + "\\GenshinImpact_Data");

                            // Paste the files in the package
                            new FastZip().ExtractZip(GlobalFilePath, GenshinDir, "");
                            Growl.Clear();
                            Growl.Success("转换成功！当前服务器：国际服 | Global");
                            key.sk("Genshin Impact Path", GenshinDir + @"\GenshinImpact.exe");
                        }
                        else
                        {
                            Directory.Move(GenshinDir + "\\GenshinImpact_Data", GenshinDir + "\\YuanShen_Data");

                            // Paste the files in the package
                            new FastZip().ExtractZip(CNFilePath, GenshinDir ,"");
                            Growl.Clear();
                            Growl.Success("转换成功！当前服务器：官方服 | 天空岛\n如果需要切换世界树 请再次切换");
                            key.sk("Genshin Impact Path", GenshinDir + @"\YuanShen.exe");
                        }
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        Growl.Clear();
                        Growl.Error("未提权 请以管理员模式打开ICora后再试");
                        Growl.Error(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        Growl.Clear();
                        Growl.Error(ex.Message);
                    }
                }
            }
            catch (DirectoryNotFoundException dirnotfound)
            {
                Growl.Clear();
                Growl.Error(dirnotfound.Message);
            }
            catch (Exception ex)
            {
                Growl.Clear();
                Growl.Error(ex.Message);
            }
        }
    }

    public enum IService
    {
        Global,
        Chinese
    }
}