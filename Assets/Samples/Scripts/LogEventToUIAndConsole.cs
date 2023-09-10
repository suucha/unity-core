using Cysharp.Threading.Tasks;
using SuuchaStudio.Unity.Core.LogEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class LogEventToUIAndConsole : LogEventReporterAbstract
{
    public override string Name => "LogEventSample";

    public LogEventToUIAndConsole()
    {
        
    }

    protected override UniTask LogEventInternal(string name, Dictionary<string, string> eventParameters)
    {

        Logger.LogDebug($"EventName:{name}\r\nEvent parameters:{Newtonsoft.Json.JsonConvert.SerializeObject(eventParameters)}");
        return UniTask.CompletedTask;
    }

}

