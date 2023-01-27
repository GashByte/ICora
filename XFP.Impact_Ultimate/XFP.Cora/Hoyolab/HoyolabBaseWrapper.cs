//Copyright (c) XFP Group and Contributors. All rights resvered.
//Licensed under the MIT License.

namespace XFP.ICora.Hoyolab
{
    public class HoyolabBaseWrapper<T> where T : class
    {
        [JsonPropertyName("returncode")]
        public int ReturnCode { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("data")]
        public T Data { get; set; }
    }
}
