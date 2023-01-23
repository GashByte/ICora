//Copyright (c) XFP Group and Contributors. All rights resvered.
//Licensed under the MIT License.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace XFP.Impact_Ultimate.Hoyolab.Account
{
    public class GenshinRoleInfoWrapper
    {
        [JsonPropertyName("list")]
        public List<GenshinRoleInfo>? List { get; set; }
    }
}
