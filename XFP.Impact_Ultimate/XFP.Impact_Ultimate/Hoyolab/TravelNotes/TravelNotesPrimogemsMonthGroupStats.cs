using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace XFP.Impact_Ultimate.Hoyolab.TravelNotes
{
    public class TravelNotesPrimogemsMonthGroupStats
    {
        [JsonIgnore]
        public int Id { get; set; }

        public int Uid { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }


        [JsonPropertyName("action_id")]
        public int ActionId { get; set; }


        [JsonPropertyName("action")]
        public string? ActionName { get; set; }


        [JsonPropertyName("num")]
        public int Number { get; set; }


        [JsonPropertyName("percent")]
        public int Percent { get; set; }
    }
}
