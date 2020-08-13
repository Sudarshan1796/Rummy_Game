using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerController : MonoBehaviour
{
    [SerializeField] private Image insideImage;
    [SerializeField] private Image borderImage;
    [SerializeField] private Image TimerPanel;
    [SerializeField] private TMP_Text timeCount;
    [SerializeField] private Color startTimeColor;
    [SerializeField] private Color midTimeColor;
    [SerializeField] private Color criticalColor;
    [SerializeField] private Color startTimeColorFade;
    [SerializeField] private Color midTimeColorFade;
    [SerializeField] private Color criticalColorFade;

    private float previosRemainingTime;
    private float remainingTime;
    private float totalTime;

    private bool isTimerActive;
    private bool isStartColorSet;
    private bool isMidColorSet;
    private bool isCriticalColorSet;

    private Action timerCompleteAction;
    private void OnEnable()
    {
        StartTimer(30, null);
    }
    public void StartTimer(float _totalTime,Action _action)
    {
        remainingTime = _totalTime;
        totalTime = remainingTime;
        previosRemainingTime = remainingTime;

        isTimerActive = true;
        isStartColorSet = false;
        isMidColorSet = false;
        isCriticalColorSet = false;

        timerCompleteAction = _action;
    }

    private void Update()
    {
        Debug.Log(isTimerActive);
        if (isTimerActive)
        {
            remainingTime = remainingTime - Time.deltaTime;
            SetImageFillAmount();
            SetTimerText();
            SetTimerColor();
            CheckTimerComplete();
        }
    }

    private void CheckTimerComplete()
    {
        if (remainingTime <= 0)
        {
            isTimerActive = false;
            timerCompleteAction?.Invoke();
            timerCompleteAction = null;
        }
    }

    private void SetImageFillAmount()
    {
        insideImage.fillAmount = remainingTime / totalTime;
        borderImage.fillAmount = remainingTime / totalTime;
    }

    private void SetTimerText()
    {
        if (previosRemainingTime - remainingTime >= 1.0f)
        {
            previosRemainingTime = remainingTime;
            timeCount.text = remainingTime.ToString("N0");
        }
    }

    private void SetTimerColor()
    {
        if (remainingTime > 15.0f && !isStartColorSet)
        {
            SetColor(ref startTimeColor, ref startTimeColorFade, ref isStartColorSet);
        }
        if (remainingTime < 15.0f && remainingTime > 7.0f && !isMidColorSet)
        {
            SetColor(ref midTimeColor, ref midTimeColorFade, ref isMidColorSet);
        }
        if (remainingTime < 7.0f && !isCriticalColorSet)
        {
            SetColor(ref criticalColor, ref criticalColorFade, ref isCriticalColorSet);
        }
    }

    private void SetColor( ref Color color,  ref Color fadeColor, ref bool issetColor)
    {
        insideImage.color = fadeColor;
        borderImage.color = color;
        TimerPanel.color = color;
        issetColor = true;
    }
}
