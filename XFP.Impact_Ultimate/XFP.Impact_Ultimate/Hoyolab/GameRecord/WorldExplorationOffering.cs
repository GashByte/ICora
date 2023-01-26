using System.Text.Json.Serialization;

namespace XFP.Impact_Ultimate.Hoyolab.GameRecord
{
    public class WorldExplorationOffering
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }


        [JsonPropertyName("level")]
        public int Level { get; set; }


        [JsonPropertyName("icon")]
        public string Icon { get; set; }
    }
}

