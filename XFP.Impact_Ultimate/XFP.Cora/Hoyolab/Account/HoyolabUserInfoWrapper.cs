//Copyright (c) XFP Group and Contributors. All rights resvered.
//Licensed under the MIT License.

namespace XFP.ICora.Hoyolab.Account
{
    public class HoyolabUserInfoWrapper
    {
        [JsonPropertyName("user_info")]
        public HoyolabUserInfo HoyolabUserInfo { get; set; }
    }
}
