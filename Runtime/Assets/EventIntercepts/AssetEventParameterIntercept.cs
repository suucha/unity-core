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

        /// <summary>
        /// Executes the asset parameter interceptor to add asset-related information to the event parameters.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="parameters">The existing event parameters.</param>
        /// <returns>A dictionary containing the updated event parameters with asset information.</returns>
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
                Logger.LogDebug($"Added asset information to event parameters for event '{eventName}': {assetEventParameterName}");

            }
            return newParameters;
        }
    }
    public interface IAssetAccessor
    {
        UniTask<List<Asset>> GetAssets();
    }
    public class SuuchaAssetAccessor : IAssetAccessor
    {
        private static SuuchaAssetAccessor instance;
        public static SuuchaAssetAccessor Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new SuuchaAssetAccessor();
                }
                return instance;
            }
        }
        public UniTask<List<Asset>> GetAssets()
        {
            var assets = Suucha.App.UserAsset.Assets;
            return UniTask.FromResult(assets);
        }
    }
}
