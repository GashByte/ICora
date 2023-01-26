using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace XFP.Impact_Ultimate.Hoyolab.TravelNotes
{
    public class TravelNotesDayData
    {
        [JsonPropertyName("current_primogems")]
        public int CurrentPrimogems { get; set; }


        [JsonPropertyName("current_mora")]
        public int CurrentMora { get; set; }


        [JsonPropertyName("last_primogems")]
        public int LastPrimogems { get; set; }


        [JsonPropertyName("last_mora")]
        public int LastMora { get; set; }
    }
}
