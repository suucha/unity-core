using Newtonsoft.Json;

namespace SuuchaStudio.Unity.Core.Assets
{
    public class Asset
    {
        public Asset()
        {
            Value = 0L;
        }
        [JsonProperty("code")]
        public string Code
        {
            get; set;
        }
        [JsonProperty("value")]
        public long Value
        {
            get; set;
        }
    }
}
