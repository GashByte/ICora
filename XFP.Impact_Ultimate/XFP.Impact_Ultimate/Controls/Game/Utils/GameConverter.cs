using HandyControl.Controls;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Description;
using System.Windows.Shapes;
using XFP.Impact_Ultimate.ICoraException;
using XFP.Impact_Ultimate.Utlis;

namespace XFP.Impact_Ultimate.Controls.Game.Utils
{
    public class GameConverter
    {
        KeySetter key = new();

        #region 库文件
        private string[] GlobalFiles = new string[]
        {
            @"\GenshinImpact.exe",
            @"\HoYoKProtect.sys",
            @"\mhypbase.dll",
            @"\pkg_version",
            @"\UnityPlayer.dll",
            @"\GenshinImpact_Data\app.info",
            @"\GenshinImpact_Data\blueReporter.exe",
            @"\GenshinImpact_Data\globalgamemanagers",
            @"\GenshinImpact_Data\globalgamemanagers.assets",
            @"\GenshinImpact_Data\globalgamemanagers.assets.resS",
            @"\GenshinImpact_Data\upload_crash.exe",
            @"\GenshinImpact_Data\Native\UserAssembly.dll",
            @"\GenshinImpact_Data\Native\UserAssembly.exp",
            @"\GenshinImpact_Data\Native\UserAssembly.lib",
            @"\GenshinImpact_Data\Plugins\Astrolabe.dll",
            @"\GenshinImpact_Data\Plugins\chrome_elf.dll",
            @"\GenshinImpact_Data\Plugins\crashreport.exe",
            @"\GenshinImpact_Data\Plugins\cri_mana_vpx.dll",
            @"\GenshinImpact_Data\Plugins\cri_vip_unity_pc.dll",
            @"\GenshinImpact_Data\Plugins\cri_ware_unity.dll",
            @"\GenshinImpact_Data\Plugins\d3dcompiler_47.dll",
            @"\GenshinImpact_Data\Plugins\hdiffz.dll",
            @"\GenshinImpact_Data\Plugins\hpatchz.dll",
            @"\GenshinImpact_Data\Plugins\kcp.dll",
            @"\GenshinImpact_Data\Plugins\libEGL.dll",
            @"\GenshinImpact_Data\Plugins\libGLESv2.dll",
            @"\GenshinImpact_Data\Plugins\libUbiCustomEvent.dll",
            @"\GenshinImpact_Data\Plugins\mailbox.dll",
            @"\GenshinImpact_Data\Plugins\MiHoYoMTRSDK.dll",
            @"\GenshinImpact_Data\Plugins\mihoyonet.dll",
            @"\GenshinImpact_Data\Plugins\MiHoYoSDKUploader.dll",
            @"\GenshinImpact_Data\Plugins\Mmoron.dll",
            @"\GenshinImpact_Data\Plugins\MTBenchmark_Windows.dll",
            @"\GenshinImpact_Data\Plugins\NamedPipeClient.dll",
            @"\GenshinImpact_Data\Plugins\Rewired_DirectInput.dll",
            @"\GenshinImpact_Data\Plugins\Telemetry.dll",
            @"\GenshinImpact_Data\Plugins\UnityNativeChromaSDK.dll",
            @"\GenshinImpact_Data\Plugins\UnityNativeChromaSDK3.dll",
            @"\GenshinImpact_Data\Plugins\vk_swiftshader.dll",
            @"\GenshinImpact_Data\Plugins\vulkan-1.dll",
            @"\GenshinImpact_Data\Plugins\xlua.dll",
            @"\GenshinImpact_Data\Plugins\ZFEmbedWeb.dll",
            @"\GenshinImpact_Data\Plugins\ZFGameBrowser.exe",
            @"\GenshinImpact_Data\Plugins\ZFProxyWeb.dll",
            @"\GenshinImpact_Data\Plugins\zf_cef.dll",
            @"\GenshinImpact_Data\StreamingAssets\20527480.blk",
            @"\GenshinImpact_Data\Managed\Metadata\global-metadata.dat",
            @"\GenshinImpact_Data\SDKCaches\telemetry\Preferences",
            @"\GenshinImpact_Data\SDKCaches\telemetry\TelemetryServiceplat_explog_sdk_v2.db",
            @"\GenshinImpact_Data\Native\Data\Metadata\global-metadata.dat",
        };

        private string[] ChineseFiles = new string[]
        {
            @"\HoYoKProtect.sys",
            @"\mhypbase.dll",
            @"\pkg_version",
            @"\UnityPlayer.dll",
            @"\YuanShen.exe",
            @"\YuanShen_Data\app.info",
            @"\YuanShen_Data\blueReporter.exe",
            @"\YuanShen_Data\globalgamemanagers",
            @"\YuanShen_Data\globalgamemanagers.assets",
            @"\YuanShen_Data\globalgamemanagers.assets.resS",
            @"\YuanShen_Data\upload_crash.exe",
            @"\YuanShen_Data\Native\UserAssembly.dll",
            @"\YuanShen_Data\Native\UserAssembly.exp",
            @"\YuanShen_Data\Native\UserAssembly.lib",
            @"\YuanShen_Data\Plugins\Astrolabe.dll",
            @"\YuanShen_Data\Plugins\chrome_elf.dll",
            @"\YuanShen_Data\Plugins\crashreport.exe",
            @"\YuanShen_Data\Plugins\cri_mana_vpx.dll",
            @"\YuanShen_Data\Plugins\cri_vip_unity_pc.dll",
            @"\YuanShen_Data\Plugins\cri_ware_unity.dll",
            @"\YuanShen_Data\Plugins\d3dcompiler_47.dll",
            @"\YuanShen_Data\Plugins\hdiffz.dll",
            @"\YuanShen_Data\Plugins\hpatchz.dll",
            @"\YuanShen_Data\Plugins\kcp.dll",
            @"\YuanShen_Data\Plugins\libEGL.dll",
            @"\YuanShen_Data\Plugins\libGLESv2.dll",
            @"\YuanShen_Data\Plugins\libUbiCustomEvent.dll",
            @"\YuanShen_Data\Plugins\mailbox.dll",
            @"\YuanShen_Data\Plugins\MiHoYoMTRSDK.dll",
            @"\YuanShen_Data\Plugins\mihoyonet.dll",
            @"\YuanShen_Data\Plugins\MiHoYoSDKUploader.dll",
            @"\YuanShen_Data\Plugins\Mmoron.dll",
            @"\YuanShen_Data\Plugins\MTBenchmark_Windows.dll",
            @"\YuanShen_Data\Plugins\NamedPipeClient.dll",
            @"\YuanShen_Data\Plugins\Rewired_DirectInput.dll",
            @"\YuanShen_Data\Plugins\Telemetry.dll",
            @"\YuanShen_Data\Plugins\UnityNativeChromaSDK.dll",
            @"\YuanShen_Data\Plugins\UnityNativeChromaSDK3.dll",
            @"\YuanShen_Data\Plugins\vk_swiftshader.dll",
            @"\YuanShen_Data\Plugins\vulkan-1.dll",
            @"\YuanShen_Data\Plugins\xlua.dll",
            @"\YuanShen_Data\Plugins\ZFEmbedWeb.dll",
            @"\YuanShen_Data\Plugins\ZFGameBrowser.exe",
            @"\YuanShen_Data\Plugins\ZFProxyWeb.dll",
            @"\YuanShen_Data\Plugins\zf_cef.dll",
            @"\YuanShen_Data\StreamingAssets\20527480.blk",
            @"\YuanShen_Data\Managed\Metadata\global-metadata.dat",
            @"\YuanShen_Data\SDKCaches\telemetry\Preferences",
            @"\YuanShen_Data\SDKCaches\telemetry\TelemetryServiceplat_explog_sdk_v2.db",
            @"\YuanShen_Data\Native\Data\Metadata\global-metadata.dat",
        };
        #endregion

        private string GenshinServiceDir = Environment.CurrentDirectory + "\\GenshinService";
        private string GenshinPath 
        { 
            get => key.gk("Genshin Impact Path");
        }

        #region 核心文件
        private readonly string CoreFile = "YuanShen.exe";
        private readonly string CoreDir = "YuanShen_Data";
        #endregion

        public void Converter()
        {
            DirectoryInfo info = new DirectoryInfo(GenshinPath);
            string GenshinDir = info.Parent.FullName;

            if (File.Exists(GenshinDir + "\\YuanShen.exe"))
            {
                ConvertGameFileAsync(IService.Global);
            }
            else if (File.Exists(GenshinDir + "\\GenshinImpact.exe"))
            {
                ConvertGameFileAsync(IService.Chinese);
            }
            else
            {
                new DirectoryNotFound("原神文件夹中的核心文件");
            }
        }

        public async Task ConvertGameFileAsync(IService Service)
        {
            try
            {

            }
            catch (DirectoryNotFoundException)
            {
                
            }
        }
    }

    public enum IService
    {
        Global,
        Chinese
    }
}