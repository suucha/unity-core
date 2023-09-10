using Cysharp.Threading.Tasks;
using SuuchaStudio.Unity.Core.LogEvents.Intercepts;
using System.Collections.Generic;

namespace SuuchaStudio.Unity.Core.LogEvents
{
    public interface ILogEventParameterIntercept
    {
        string Name { get; }
        int Order { get; }
        UniTask<Dictionary<string, string>> Execute(string eventName, Dictionary<string, string> parameters);

    }
}
