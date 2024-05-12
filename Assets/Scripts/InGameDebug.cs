using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InGameDebug : MonoBehaviour {

    List<string> logLines;
    public TextMeshProUGUI debugLabel;

    void Awake () {
        logLines = new List<string>();
    }
	
    public void Log(string newLog) {
        logLines.Insert(0, newLog);
        if (logLines.Count > 20) {
            logLines.RemoveAt(logLines.Count - 1);
        }
        debugLabel.text = "";
        for (int i = 0; i < logLines.Count; i++)
            debugLabel.text += "\n" + logLines[i];
    }


    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        Log(logString + "\n" + stackTrace);
    }

}
