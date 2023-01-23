//Copyright (c) XFP Group and Contributors. All rights resvered.
//Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using XFP.Impact_Ultimate.Hoyolab.Account;
using XFP.Impact_Ultimate.ICoraException;

namespace XFP.Impact_Ultimate.Hoyolab
{
    public class HoyolabClient
    {
        #region Header
        private const string Accept = "Accept";
        private const string Cookie = "Cookie";
        private const string UserAgent = "User-Agent";
        private const string X_Reuqest_With = "X-Requested-With";
        private const string DS = "DS";
        private const string Referer = "Referer";
        private const string Application_Json = "application/json";
        private const string com_mihoyo_hyperion = "com.mihoyo.hyperion";
        private const string x_rpc_app_version = "x-rpc-app_version";
        private const string x_rpc_device_id = "x-rpc-device_id";
        private const string x_rpc_client_type = "x-rpc-client_type";
        private const string UAContent = $"Mozilla/5.0 miHoYoBBS/{AppVersion}";
        private const string AppVersion = "2.35.2";
        private static readonly string DeviceId = Guid.NewGuid().ToString("D");
        #endregion

        private readonly HttpClient httpClient;

        /// <summary>
        /// 构造一个新的hoyolabclient
        /// </summary>
        /// <param name="httpClient"></param>
        public HoyolabClient(HttpClient? httpClient = null)
        {
            this.httpClient = httpClient ?? new(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.All });
        }

        private async Task<T> CommonSendAsync<T>(
            HttpRequestMessage request,
            CancellationToken? cancellationToken = null) where T : class
        {
            request.Headers.Add(Accept, Application_Json);
            request.Headers.Add(UserAgent, UAContent);
            var response = await httpClient.SendAsync(request, cancellationToken ?? CancellationToken.None);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var responseData = JsonSerializer.Deserialize<HoyolabBaseWrapper<T>>(content);
            if (responseData is null)
            {
                throw new HoyolabException(-1, "Can not parse the response body.");
            }
            if (responseData.ReturnCode != 0)
            {
                throw new HoyolabException(responseData.ReturnCode, responseData.Message);
            }
            return responseData.Data;
        }

        /// <summary>
        /// 米游社账号信息
        /// </summary>
        /// <param name="cookie"></param>
        public async Task<HoyolabUserInfo> GetHoyolabUserInfoAsync(string cookie, CancellationToken? cancellationToken = null)
        {
            if (string.IsNullOrWhiteSpace(cookie))
            {
                new ContentEmptyException().ContentIsEmpty(cookie, -1);
            }
            var request = new HttpRequestMessage(HttpMethod.Get, "https://bbs-api.mihoyo.com/user/wapi/getUserFullInfo?gids=2");
            request.Headers.Add(Cookie, cookie);
            request.Headers.Add(Referer, "https://bbs.mihoyo.com/");
            request.Headers.Add(DS, DynamicSecret.CreateSecret());
            request.Headers.Add(x_rpc_app_version, AppVersion);
            request.Headers.Add(x_rpc_device_id, DeviceId);
            request.Headers.Add(x_rpc_client_type, "5");
            var data = await CommonSendAsync<HoyolabUserInfoWrapper>(request, cancellationToken);
            data.HoyolabUserInfo.Cookie = cookie;
            return data.HoyolabUserInfo;
        }

        /// <summary>
        /// 获取原神账号信息
        /// </summary>
        /// <param name="cookie"></param>
        public async Task<List<GenshinRoleInfo>> GetGenshinRoleInfoListAsync(string cookie, CancellationToken? cancellationToken = null)
        {
            if (string.IsNullOrWhiteSpace(cookie))
            {
                new ContentEmptyException().ContentIsEmpty(cookie, -1);
            }
            var url = "https://api-takumi.mihoyo.com/binding/api/getUserGameRolesByCookie?game_biz=hk4e_cn";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add(Cookie, cookie);
            request.Headers.Add(DS, DynamicSecret.CreateSecret2(url));
            request.Headers.Add(X_Reuqest_With, com_mihoyo_hyperion);
            request.Headers.Add(x_rpc_app_version, AppVersion);
            request.Headers.Add(x_rpc_client_type, "5");
            request.Headers.Add(Referer, "https://webstatic.mihoyo.com/app/community-game-records/?game_id=2&utm_source=bbs&utm_medium=mys&utm_campaign=box");
            var data = await CommonSendAsync<GenshinRoleInfoWrapper>(request, cancellationToken);
            data.List?.ForEach(x => x.Cookie = cookie);
            return data.List ?? new List<GenshinRoleInfo>();
        }
    }
}
