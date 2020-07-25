using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AndroidVibrationPlugin
{
    const string pluginName = "unity.vibrator.yudiz.vibrator.UnityVibrator";
    static AndroidJavaClass _pluginClass;
    static AndroidJavaObject _pluginObject;
    static AndroidJavaObject _activityContext;
    public static AndroidJavaClass PluginClass
    {
        get
        {
            if (_pluginClass == null)
            {
                _pluginClass = new AndroidJavaClass(pluginName);
                Debug.Log("Plugin class created");
            }
            return _pluginClass;
        }
    }

    public static AndroidJavaObject ActivityContext
    {
        get
        {
            if (_activityContext == null)
            {
                var actClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                _activityContext = actClass.GetStatic<AndroidJavaObject>("currentActivity");
            }
            return _activityContext;
        }
    }

    public static AndroidJavaObject PluginObject
    {
        get
        {
            if (_pluginObject == null)
            {
                _pluginObject = PluginClass.CallStatic<AndroidJavaObject>("getInstance");
                _pluginObject.Call("SetContext", ActivityContext);
            }
            return _pluginObject;
        }
    }
    public static void Vibrate(int time, int amplitude)
    {
        PluginObject.Call("Vibrate", time, amplitude);
    }
}
