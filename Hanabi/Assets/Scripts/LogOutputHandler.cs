using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class LogOutputHandler : MonoBehaviour {

    //Register the HandleLog function on scene start to fire on debug.log events
    public void OnEnable() {
        Application.logMessageReceived += HandleLog;
    }

    //Remove callback when object goes out of scope
    public void OnDisable() {
        Application.logMessageReceived -= HandleLog;
    }

    //Create a string to store log level in
    string level = "";

    //Capture debug.log output, send logs to Loggly
    public void HandleLog(string logString, string stackTrace, LogType type) {

        //Initialize WWWForm and store log level as a string
        level = type.ToString();
        var loggingForm = new WWWForm();

        //Add log message to WWWForm
        loggingForm.AddField("LEVEL", level);
        //loggingForm.AddField("Stack_Trace", stackTrace);

        //Add any User, Game, or Device MetaData that would be useful to finding issues later
        //loggingForm.AddField("Device_Model", SystemInfo.deviceModel);
        loggingForm.AddField("Player", PlayerPrefs.GetString("uid"));
        loggingForm.AddField("Time", DateTime.Now.ToString("HH:mm:ss.FFF"));
        loggingForm.AddField("Message", logString);

        StartCoroutine(SendData(loggingForm));
    }

    public IEnumerator SendData(WWWForm form) {
        using (UnityWebRequest www = UnityWebRequest.Post("http://logs-01.loggly.com/inputs/ec76e028-01be-4652-99c6-e7f67eaeffb9/tag/Unity3D", form)) {
            yield return www.SendWebRequest();
        }
    }
}