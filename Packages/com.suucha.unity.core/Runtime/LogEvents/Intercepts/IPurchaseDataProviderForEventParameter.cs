using Cysharp.Threading.Tasks;

namespace SuuchaStudio.Unity.Core.LogEvents.Intercepts
{
    public interface IPurchaseDataProviderForEventParameter
    {
        UniTask<float> GetCumulativeValue();
        UniTask<float> GetValue();
    }
}
