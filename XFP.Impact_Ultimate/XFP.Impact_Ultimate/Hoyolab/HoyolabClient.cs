//Copyright (c) XFP Group and Contributors. All rights resvered.
//Licensed under the MIT License.

using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using XFP.Impact_Ultimate.Hoyolab.Account;
using XFP.Impact_Ultimate.Hoyolab.DailyNote;
using XFP.Impact_Ultimate.Hoyolab.GameRecord;
using XFP.Impact_Ultimate.Hoyolab.TravelNotes;
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
        /// 更新米游社数据
        /// </summary>
        /// <returns></returns>
        public async Task UpdateAllAccountsAsync()
        {
            await GetHoyolabUserInfoAsync(Properties.Settings.Default.UserCookie!);
            await GetGenshinRoleInfoListAsync(Properties.Settings.Default.UserCookie!);
        }

        public async Task<bool> SignInAsync(GenshinRoleInfo role, bool skipCheckWhetherHaveSignedIn = false, CancellationToken? cancellationToken = null)
        {
            if (!skipCheckWhetherHaveSignedIn)
            {
                var signInfo = await GetSignInInfoAsync(role, cancellationToken);
                if (signInfo.IsSign)
                {
                    return false;
                }
            }
            var obj = new { act_id = "e202009291139501", region = role.Region.ToString(), uid = role.Uid.ToString() };
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api-takumi.mihoyo.com/event/bbs_sign_reward/sign");
            request.Headers.Add(Cookie, role.Cookie);
            request.Headers.Add(DS, DynamicSecret.CreateSecret());
            request.Headers.Add(x_rpc_app_version, AppVersion);
            request.Headers.Add(x_rpc_device_id, DeviceId);
            request.Headers.Add(x_rpc_client_type, "5");
            request.Headers.Add(X_Reuqest_With, com_mihoyo_hyperion);
            request.Headers.Add(Referer, "https://webstatic.mihoyo.com/bbs/event/signin-ys/index.html?bbs_auth_required=true&act_id=e202009291139501&utm_source=bbs&utm_medium=mys&utm_campaign=icon");
            request.Content = JsonContent.Create(obj);
            var risk = await CommonSendAsync<SignInRisk>(request, cancellationToken);
            if (risk is null or { RiskCode: 0, Success: 0 })
            {
                return true;
            }
            else
            {
                Growl.Clear();
                Growl.Error($"账户 {role.Nickname} 受到风控限制 请手动签到");
                return false;
            }
        }

        /// <summary>
        /// 实时便笺
        /// </summary>
        /// <param name="role"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<DailyNoteInfo> GetDailyNoteAsync(GenshinRoleInfo role, CancellationToken? cancellationToken = null)
        {
            var url = $"https://api-takumi-record.mihoyo.com/game_record/app/genshin/api/dailyNote?server={role.Region}&role_id={role.Uid}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add(Cookie, role.Cookie);
            request.Headers.Add(DS, DynamicSecret.CreateSecret2(url));
            request.Headers.Add(Referer, "https://webstatic.mihoyo.com/app/community-game-records/?game_id=2&utm_source=bbs&utm_medium=mys&utm_campaign=box");
            request.Headers.Add(x_rpc_app_version, AppVersion);
            request.Headers.Add(x_rpc_client_type, "5");
            request.Headers.Add(X_Reuqest_With, com_mihoyo_hyperion);
            var data = await CommonSendAsync<DailyNoteInfo>(request);
            data.Uid = role.Uid;
            data.Nickname = role.Nickname;
            return data;
        }

        /// <summary>
        /// 旅行札记总览
        /// </summary>
        /// <param name="role"></param>
        /// <param name="month">0 当前月</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<TravelNotesSummary> GetTravelNotesSummaryAsync(GenshinRoleInfo role, int month = 0, CancellationToken? cancellationToken = null)
        {
            var url = $"https://hk4e-api.mihoyo.com/event/ys_ledger/monthInfo?month={month}&bind_uid={role.Uid}&bind_region={role.Region}&bbs_presentation_style=fullscreen&bbs_auth_required=true&utm_source=bbs&utm_medium=mys&utm_campaign=icon";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add(Cookie, role.Cookie);
            request.Headers.Add(Referer, "https://webstatic.mihoyo.com/ys/event/e20200709ysjournal/index.html?bbs_presentation_style=fullscreen&bbs_auth_required=true&utm_source=bbs&utm_medium=mys&utm_campaign=icon");
            request.Headers.Add(X_Reuqest_With, com_mihoyo_hyperion);
            return await CommonSendAsync<TravelNotesSummary>(request);
        }

        /// <summary>
        /// 账号的签到信息
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<SignInInfo> GetSignInInfoAsync(GenshinRoleInfo role, CancellationToken? cancellationToken = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api-takumi.mihoyo.com/event/bbs_sign_reward/info?act_id=e202009291139501&region={role.Region}&uid={role.Uid}");
            request.Headers.Add(Cookie, role.Cookie);
            request.Headers.Add(x_rpc_device_id, DeviceId);
            request.Headers.Add(X_Reuqest_With, com_mihoyo_hyperion);
            request.Headers.Add(Referer, "https://webstatic.mihoyo.com/bbs/event/signin-ys/index.html?bbs_auth_required=true&act_id=e202009291139501&utm_source=bbs&utm_medium=mys&utm_campaign=icon");
            return await CommonSendAsync<SignInInfo>(request, cancellationToken);
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

        /// <summary>
        /// 获取用户战绩
        /// </summary>
        /// <param name="role"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<GameRecordSummary> GetGameRecordAsync(GenshinRoleInfo role, CancellationToken? cancellationToken = null)
        {
            var url = $"https://api-takumi-record.mihoyo.com/game_record/app/genshin/api/index?server={role.Region}&role_id={role.Uid}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add(Cookie, role.Cookie);
            request.Headers.Add(DS, DynamicSecret.CreateSecret2(url));
            request.Headers.Add(Referer, "https://webstatic.mihoyo.com/app/community-game-records/?game_id=2&utm_source=bbs&utm_medium=mys&utm_campaign=box");
            request.Headers.Add(x_rpc_app_version, AppVersion);
            request.Headers.Add(x_rpc_client_type, "5");
            request.Headers.Add(X_Reuqest_With, com_mihoyo_hyperion);
            return await CommonSendAsync<GameRecordSummary>(request);
        }
    }
}
