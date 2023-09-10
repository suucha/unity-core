using Cysharp.Threading.Tasks;

namespace SuuchaStudio.Unity.Core.AdPlaying
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAdPlayerManager
    {
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <returns></returns>
        UniTask<bool> Initialize();
    }
}
