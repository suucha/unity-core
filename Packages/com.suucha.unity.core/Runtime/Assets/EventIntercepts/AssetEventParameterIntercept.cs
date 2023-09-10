using SuuchaStudio.Unity.Core.LogEvents;
using Newtonsoft.Json;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace SuuchaStudio.Unity.Core.Assets.EventIntercepts
{
    public class AssetEventParameterIntercept : SuuchaBase, ILogEventParameterIntercept
    {
        private readonly IAssetAccessor assetAccessor;
        private readonly string assetEventParameterName;
        public AssetEventParameterIntercept(IAssetAccessor assetAccessor, string assetEventParameterName)
        {
            this.assetAccessor = assetAccessor;
            this.assetEventParameterName = assetEventParameterName;
        }
        public string Name => "AssetEventIntercept";
        public int Order => 10;

        public async UniTask<Dictionary<string, string>> Execute(string eventName, Dictionary<string, string> parameters)
        {
            Dictionary<string, string> newParameters; 
            if (parameters != null)
            {
                newParameters = new Dictionary<string, string>(parameters);
            }
            else
            {
                newParameters = new Dictionary<string, string>();
            }

            if (string.IsNullOrEmpty(assetEventParameterName))
            {
                return newParameters;
            }
            var assets = await assetAccessor.GetAssets();
            if (assets != null && assets.Count > 0)
            {
                var assetsJson = JsonConvert.SerializeObject(assets);
                newParameters.Add(assetEventParameterName, assetsJson);
            }
            return newParameters;
        }
    }
    public interface IAssetAccessor
    {
        UniTask<List<Asset>> GetAssets();
    }
}
