using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public delegate void OnTimerFinished();

public class Timer : MonoBehaviour
{
    // directly set the on finished delegate in other scripts
    public OnTimerFinished onTimerFinished;
    public bool finished;

    public float countdown;

    [SerializeField] float step;
    [SerializeField] float elapsed;
    [SerializeField] TextMeshProUGUI display;


    void Awake()
    {
        finished = false;
        ResetTimer();
    }

    public void ResetTimer()
    {
        finished = false;
        elapsed = countdown;
    }

    public void StartTimer()
    {
        UpdateDisplay();
        StartCoroutine(Countdown(onTimerFinished));
    }

    public void StopTimer()
    {
        StopAllCoroutines();
    }

    public float TimeElapsed()
    {
        return countdown - elapsed;
    }


    void UpdateDisplay()
    {
        if (display) 
            display.text = string.Format("Time Remaining {0:00}:{1:00}", 
                                            Mathf.FloorToInt(elapsed) / 60, Mathf.FloorToInt(elapsed) % 60);
    }

    IEnumerator Countdown(OnTimerFinished onTimerFinished)
    {
        WaitForSeconds wait = new WaitForSeconds(step);
        while (elapsed > 0)
        {
            // show countdown
            elapsed -= step;
            yield return wait;
            UpdateDisplay();
        }
        onTimerFinished();
        finished = true;
        yield break;
    }

}
