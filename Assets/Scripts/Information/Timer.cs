using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField, Range(1, 99)] int timeLeft;
    [SerializeField, Range(1, 10)] int timeScale;
    [SerializeField] Text timeLeftText;

    bool stopTimer;
    float _timeLeft;

    public event Action TimeOver;

    private void Start()
    {
        stopTimer = true;
        timeLeftText.text = timeLeft.ToString();
    }

    public void StartTimer()
    {
        // timeScaleを大きくするとtimeLeftの数値である時間が短いので+1しておく
        _timeLeft = (timeLeft + 1) * timeScale;
        stopTimer = false;
    }

    public void StopTimer()
    {
        stopTimer = true;
    }

    private void FixedUpdate()
    {
        if (stopTimer)
        {
            return;
        }
        _timeLeft -= Time.deltaTime;
        if(_timeLeft > 0)
        {
            timeLeftText.text = ((int)(_timeLeft / timeScale)).ToString();
        } else
        {
            stopTimer = true;
            if (TimeOver != null)
            {
                TimeOver();
            }
        }
    }
}
