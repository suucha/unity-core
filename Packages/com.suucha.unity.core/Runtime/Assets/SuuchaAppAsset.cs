using Newtonsoft.Json;
using SuuchaStudio.Unity.Core.Assets;
using SuuchaStudio.Unity.Core.Assets.EventIntercepts;
using SuuchaStudio.Unity.Core.LogEvents;
using System.Collections.Generic;
namespace SuuchaStudio.Unity.Core
{
    public partial class Suucha
    {
        private UserAsset userAsset;
        private AssetChangedEventGenerator assetChangedEventGenerator = null;
        private readonly string assetsLocalStoragePrefixKey = "SuuchaAssetsLocalStorageKey";
        private readonly string suuchaAssetEventParameterName = "suc_assets";
        public UserAsset UserAsset
        {
            get
            {
                var userAsset = GetUserAssetFromLocalStorage();
                return userAsset;
            }
        }
        private UserAsset GetUserAssetFromLocalStorage()
        {
            if (userAsset != null)
            {
                return userAsset;
            }
            userAsset = LocalStorage.Get<UserAsset>(assetsLocalStoragePrefixKey);
            if (userAsset == null)
            {
                userAsset = new UserAsset();
                LocalStorage.Set<UserAsset>(assetsLocalStoragePrefixKey, userAsset);
            }
            return userAsset;
        }
        private void SaveUserAssetToLocalStorage()
        {
            if (userAsset == null)
            {
                return;
            }
            var key = assetsLocalStoragePrefixKey;
            Logger.LogDebug($"Save userAssets:Key:{key},{JsonConvert.SerializeObject(userAsset)}");
            LocalStorage.Set(key, userAsset);
        }
        public long AddAsset(string assetCode, long value)
        {
            var asset = GetUserAssetFromLocalStorage();
            var banlance = asset.AddAsset(assetCode, value);
            SaveUserAssetToLocalStorage();
            return banlance;
        }
        public long SubAsset(string assetCode, long value)
        {
            var asset = GetUserAssetFromLocalStorage();
            var banlance = asset.SubAsset(assetCode, value);
            SaveUserAssetToLocalStorage();
            return banlance;
        }
        public long GetAsset(string assetCode)
        {
            var asset = GetUserAssetFromLocalStorage();
            var banlance = asset.GetAsset(assetCode);
            return banlance;
        }
        public void EnableAssetChangedEvent(List<AssetChangedEventConfiguration> assetChangedEventConfigurations)
        {
            if (assetChangedEventGenerator != null && this.userAsset != null)
            {
                this.userAsset.AssetIncreased -= assetChangedEventGenerator.AssetIncreasedCallback;
            }
            var userAsset = GetUserAssetFromLocalStorage();
            assetChangedEventGenerator = new AssetChangedEventGenerator(assetChangedEventConfigurations);
            userAsset.AssetIncreased += assetChangedEventGenerator.AssetIncreasedCallback;
        }
        public void EnableAssetEventParameter(string assetEventParameterName, IAssetAccessor assetAccessor)
        {
            if (string.IsNullOrEmpty(assetEventParameterName))
            {
                assetEventParameterName = suuchaAssetEventParameterName;
            }
            var intercept = new AssetEventParameterIntercept(assetAccessor, assetEventParameterName);
            LogEventManager.AddEventParameterInterceptor(intercept);
        }
    }
}
