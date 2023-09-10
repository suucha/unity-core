using SuuchaStudio.Unity.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogEvent : SuuchaMonoBehaviourBase
{
    public Text EventText;
    // Start is called before the first frame update
    void Start()
    {
        Logger.LogDebug("");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
