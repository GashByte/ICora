//Copyright (c) XFP Group and Contributors. All rights resvered.
//Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading.Tasks;
using XFP.Impact_Ultimate.Hoyolab.Account;

namespace XFP.Impact_Ultimate.Hoyolab.Service
{
    public class HoyolabService
    {
        private HoyolabClient hoyolabClient;

        public HoyolabService(HoyolabClient hoyolabClient)
        {
            this.hoyolabClient = hoyolabClient;
        }

        public async Task<HoyolabUserInfo> GetHoyolabUserInfoAsync(string cookie)
        {
            var user = await hoyolabClient.GetHoyolabUserInfoAsync(cookie);
            return user;
        }

        public async Task<List<GenshinRoleInfo>> GetGenshinRoleInfoListAsync(string cookie)
        {
            var roles = await hoyolabClient.GetGenshinRoleInfoListAsync(cookie);
            return roles;
        }
    }
}
