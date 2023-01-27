//Copyright (c) XFP Group and Contributors. All rights resvered.
//Licensed under the MIT License.

namespace XFP.ICora.Hoyolab.Account
{
    public class GenshinRoleInfoWrapper
    {
        [JsonPropertyName("list")]
        public List<GenshinRoleInfo>? List { get; set; }
    }
}
