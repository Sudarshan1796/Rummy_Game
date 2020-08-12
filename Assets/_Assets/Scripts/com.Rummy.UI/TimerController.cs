using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerController : MonoBehaviour
{
    [SerializeField] private Image image;

    private float remainingTime;
    private float totalTime;

    private Coroutine _updateCoroutine;
    public void StartTimer(float _totalTime,Action _action)
    {
        remainingTime = _totalTime;
        totalTime = remainingTime;
        if(_updateCoroutine!=null)
        {
            StopCoroutine(_updateCoroutine);
        }
        _updateCoroutine = StartCoroutine(UpdateTimer(_action));
    }

    private IEnumerator UpdateTimer(Action _action)
    {
        image.fillAmount = remainingTime / totalTime;
        yield return new WaitForSeconds(1.0f);
        remainingTime--;
        if(remainingTime>0)
        {
            _updateCoroutine = StartCoroutine(UpdateTimer(_action));
        }
        else
        {
            _action.Invoke();
        }
    }
}
