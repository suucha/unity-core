using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SuuchaStudio.Unity.Core.LogEvents
{
    public class AppRunDurationEventParameterIntercept : SuuchaBase, ILogEventParameterIntercept
    {
        public string Name => "AppRunDuration";

        public int Order => 100;

        public UniTask<Dictionary<string, string>> Execute(string eventName, Dictionary<string, string> parameters)
        {
            return UniTask.FromResult(new Dictionary<string, string>() { { EventParameterNames.AppRunDuration, ((long)(UnityEngine.Time.realtimeSinceStartup * 1000)).ToString() } });
        }
    }
}
