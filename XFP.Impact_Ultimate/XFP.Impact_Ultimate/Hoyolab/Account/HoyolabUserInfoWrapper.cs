//Copyright (c) XFP Group and Contributors. All rights resvered.
//Licensed under the MIT License.

using System.Text.Json.Serialization;

namespace XFP.Impact_Ultimate.Hoyolab.Account
{
    public class HoyolabUserInfoWrapper
    {
        [JsonPropertyName("user_info")]
        public HoyolabUserInfo HoyolabUserInfo { get; set; }
    }
}
