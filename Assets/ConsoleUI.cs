using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ConsoleUI : MonoBehaviour
{
    static string myLog = "";
    private string output;
    private string stack;


    public void Log(string logString, string stackTrace, LogType type)
    {
        output = logString;
        stack = stackTrace;
        myLog = output + "\n" + myLog;
        if (myLog.Length > 10000)
        {
            myLog = myLog.Substring(0, 10000);
        }
    }


    #region [Unity Methods]

    void OnEnable()
    {
        Application.logMessageReceived += Log;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= Log;
    }

    void OnGUI()
    {
        // if (!Application.isEditor) //Do not display in editor ( or you can use the UNITY_EDITOR macro to also disable the rest)
        {
            myLog = GUI.TextArea(new Rect(10, 10, Screen.width - 10, Screen.height - 10), myLog);
        }
    }

    #endregion
}
