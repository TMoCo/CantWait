using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnTick(float c);

public class StopWatch : MonoBehaviour
{
    public float step = 1.0f;
    public float count = 0.0f;
    public OnTick onTick;

    public void ResetCount()
    {
        StopCount();
        count = 0.0f;
    }

    public void StartCount()
    {
        StartCoroutine("Count");
    }

    public void StopCount()
    {
        StopCoroutine("Count");
    }

    IEnumerator Count()
    {
        WaitForSeconds wait = new WaitForSeconds(step);
        while (true)
        {
            // show countdown
            ++count;
            yield return wait;
            onTick(count);
        }
    }
}
