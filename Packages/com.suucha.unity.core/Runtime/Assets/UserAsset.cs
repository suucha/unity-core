using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace SuuchaStudio.Unity.Core.Assets
{
    public delegate void AssetIncreasedHandler(string code, long newValue, long oldValue);
    public delegate void AssetReducedHandler(string code, long newValue, long oldValue);
    public class UserAsset
    {
        public event AssetIncreasedHandler AssetIncreased;
        public event AssetReducedHandler AssetReduced;
        public UserAsset()
        {
            Assets = new List<Asset>();
        }
        private string userId;
        [JsonProperty("userId")]
        public string UserId
        {
            get
            {
                return userId;
            }
            set
            {
                userId = value;
            }
        }
        [JsonProperty("assets")]
        public List<Asset> Assets { get; set; }

        public long GetAsset(string code)
        {
            var asset = Assets.FirstOrDefault(a => a.Code == code);
            if (asset == null)
            {
                return 0L;
            }
            return asset.Value;

        }
        public long AddAsset(string code, long value, string source = "", string sourceId = "")
        {
            var asset = Assets.FirstOrDefault(a => a.Code == code);
            if (asset == null)
            {
                asset = new Asset
                {
                    Code = code,
                    Value = 0L
                };
                Assets.Add(asset);
            }

            var oldValue = asset.Value;
            asset.Value += value;
            if (value > 0)
            {
                AssetIncreased?.Invoke(code, asset.Value, oldValue);
            }
            return asset.Value;
        }

        public long SubAsset(string code, long value, string source = "", string sourceId = "")
        {
            var asset = Assets.FirstOrDefault(a => a.Code == code);
            if (asset == null)
            {
                asset = new Asset
                {
                    Code = code,
                    Value = 0L
                };
                Assets.Add(asset);
            }
            var oldValue = asset.Value;
            asset.Value -= value;
            if (value != 0)
            {
                AssetReduced?.Invoke(code, asset.Value, oldValue);
            }
            return asset.Value;
        }
    }
}
