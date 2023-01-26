using Newtonsoft.Json;

namespace XFP.Impact_Ultimate.Hoyolab.Account
{
    public class IWrapper
    {
        /// <summary>
        /// 用户Uid
        /// </summary>
        [JsonProperty("Uid")]
        public string Uid { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        [JsonProperty("Name")]
        public string NickName { get; set; }

        /// <summary>
        /// 用户等级
        /// </summary>
        [JsonProperty("Level")]
        public string Level { get; set; }

        /// <summary>
        /// 用户Cookie
        /// </summary>
        [JsonProperty("Cookie")]
        public string Cookie { get; set; }

        /// <summary>
        /// GameBiz
        /// </summary>
        [JsonProperty("GameBiz")]
        public string GameBiz { get; set; }

        /// <summary>
        /// IsChosen
        /// </summary>
        [JsonProperty("IsChosen")]
        public string IsChosen { get; set; }

        /// <summary>
        /// IsOfficial
        /// </summary>
        [JsonProperty("IsOfficial")]
        public string IsOfficial { get; set; }

        /// <summary>
        /// 服务器名称
        /// </summary>
        [JsonProperty("RegionName")]
        public string RegionName { get; set; }

        /// <summary>
        /// 服务器
        /// </summary>
        [JsonProperty("Region")]
        public string Region { get; set; }
    }
}
