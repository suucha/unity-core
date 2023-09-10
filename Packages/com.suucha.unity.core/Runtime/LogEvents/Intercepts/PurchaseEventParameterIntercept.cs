using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Globalization;

namespace SuuchaStudio.Unity.Core.LogEvents.Intercepts
{
    public class PurchaseEventParameterIntercept : SuuchaBase, ILogEventParameterIntercept
    {
        public string Name => "PurchaseEventParameter";

        public int Order => 100;
        private readonly IPurchaseDataProviderForEventParameter purchaseDataProvider;
        public PurchaseEventParameterIntercept(IPurchaseDataProviderForEventParameter purchaseDataProvider)
        {
            this.purchaseDataProvider = purchaseDataProvider ?? throw new System.InvalidOperationException("purchaseDataProvider is null");
        }
        public async UniTask<Dictionary<string, string>> Execute(string eventName, Dictionary<string, string> parameters)
        {
            var newParameters = new Dictionary<string, string>();
            if (purchaseDataProvider != null)
            {
                var value = await purchaseDataProvider.GetCumulativeValue();
                newParameters.Add(EventParameterNames.EventCumulativePurchase, value.ToString("F10", CultureInfo.InvariantCulture));
                value = await purchaseDataProvider.GetValue();
                newParameters.Add(EventParameterNames.EventPurchase, value.ToString("F10", CultureInfo.InvariantCulture));
            }
            return newParameters;
        }
    }
}
