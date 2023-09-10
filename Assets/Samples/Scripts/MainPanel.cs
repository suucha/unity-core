using SuuchaStudio.Unity.Core;
using SuuchaStudio.Unity.Core.LogEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : SuuchaMonoBehaviourBase
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnLogEventClick()
    {
        Suucha.App.LogEvent("test");
        Suucha.App.LogEvent("click");
        Suucha.App.LogEvent("spin");
    }

    public void OnLoggingClick()
    {
        Logger.LogDebug("OnLoggingClick");
    }
}
