namespace XFP.ICora.Hoyolab.DailyNote
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
