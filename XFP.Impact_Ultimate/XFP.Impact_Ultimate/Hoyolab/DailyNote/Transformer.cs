using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace XFP.Impact_Ultimate.Hoyolab.DailyNote
{
    public class Transformer
    {
        /// <summary>
        /// 是否获得
        /// </summary>
        [JsonPropertyName("obtained")]
        public bool Obtained { get; set; }

        /// <summary>
        /// 剩余时间
        /// </summary>
        [JsonPropertyName("recovery_time")]
        public TransformerRecoveryTime RecoveryTime { get; set; }

        /// <summary>
        /// Wiki url
        /// </summary>
        [JsonPropertyName("wiki")]
        public string Wiki { get; set; }
    }
}
