using SuuchaStudio.Unity.Core;
using SuuchaStudio.Unity.Core.Assets.EventIntercepts;
using SuuchaStudio.Unity.Core.LogEvents;
using SuuchaStudio.Unity.Core.LogEvents.Intercepts;

public class AfterSuuchaInit : SuuchaBase, IAfterSuuchaInit
{
    public void Execute()
    {
        Suucha.App.AddLogEventIntercept(new EventCounterIntercept(new System.Collections.Generic.List<EventCounterConfiguration>
        {
            new EventCounterConfiguration()
            {
                Name="test",
                CountList = new System.Collections.Generic.List<int> { 1, 2,3,5,10 }
            },
            new EventCounterConfiguration()
            {
                Name="click",
                CountList = new System.Collections.Generic.List<int> { 1, 2,3,5,10 }
            }
        }));
        Suucha.App.AddLogEventParameterIntercept(new AppRunDurationEventParameterIntercept());
        Suucha.App.AddLogEventParameterIntercept(new EventCountParameterIntercept(UnityEngine.Application.persistentDataPath));
        //Suucha.App.AddLogEventParameterIntercept(new AssetEventParameterIntercept());
        Suucha.App.AddLogEventReporter(new LogEventToUIAndConsole())
            .EnableEventCounter();
    }
}

