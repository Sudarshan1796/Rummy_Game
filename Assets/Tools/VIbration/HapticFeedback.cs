using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;
public enum UIFeedbackType{
    Selection = 0,
    ImpactLight,
    ImpactMedium,
    ImpactHeavy,
    Success,
    Warning,
    Error
}
public class HapticFeedback 
{
    public static IVibrator vibrator;

    public static void Generate()
    {
        if (vibrator == null)
        {
            if (vibrator == null)
            {
#if UNITY_EDITOR
                return;
#elif UNITY_ANDROID
                vibrator = new IAndroidVibrator();
#elif UNITY_IOS
                vibrator = new IPhoneVibrator();
#endif
            }
        }
    }

    public static void Vibrate(UIFeedbackType type)
    {
        if (vibrator != null)
        {
            vibrator.Vibrate(type);
        }
    }

    public static void SetVibrationOn(bool on)
    {
        if(on)
        {
            Generate();
        }
        else
        {
            vibrator = null;
        }
    }
}




public interface IVibrator
{
    void Vibrate(UIFeedbackType type);
}


#if UNITY_IOS

public class IPhoneVibrator:IVibrator
{
    public void Vibrate(UIFeedbackType type)
    {
        GenerateFeedback((int)type);
    }
    [DllImport("__Internal")]
    private static extern void GenerateFeedback(int type);
}
#endif

#if UNITY_ANDROID
public class IAndroidVibrator : IVibrator
{
    public void Vibrate(UIFeedbackType type)
    {
        switch (type)
        {
            case UIFeedbackType.ImpactLight:
                AndroidVibrationPlugin.Vibrate(30, 160);
                break;
            case UIFeedbackType.ImpactMedium:
                AndroidVibrationPlugin.Vibrate(30, 210);
                break;
            case UIFeedbackType.ImpactHeavy:
                AndroidVibrationPlugin.Vibrate(50, 255);
                break;
        }
    }
}
#endif