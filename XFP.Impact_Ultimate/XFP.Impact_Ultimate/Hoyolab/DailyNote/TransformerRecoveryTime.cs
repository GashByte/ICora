using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace XFP.Impact_Ultimate.Hoyolab.DailyNote
{
    public class TransformerRecoveryTime
    {
        [JsonPropertyName("Day")]
        public int Day { get; set; }

        [JsonPropertyName("Hour")]
        public int Hour { get; set; }

        [JsonPropertyName("Minute")]
        public int Minute { get; set; }

        [JsonPropertyName("Second")]
        public int Second { get; set; }

        /// <summary>
        /// 是否可再次使用
        /// </summary>
        [JsonPropertyName("reached")]
        public bool Reached { get; set; }
    }
}
