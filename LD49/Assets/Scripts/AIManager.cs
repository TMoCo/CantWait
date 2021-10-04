using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    [SerializeField] AIController[] aIControllers;

    bool paused = false;

    // Start is called before the first frame update
    void Start()
    {
        foreach(AIController ctrller in aIControllers)
        {
            ctrller.ResetAI();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!paused)
            foreach (AIController ctrller in aIControllers)
                if (!ctrller.waiting)
                    ctrller.UpdateAI();
    }

    public void PauseAIControllers(bool pause)
    {
        paused = pause;
        if (paused) // pause means stop timers
            foreach (AIController ctrller in aIControllers)
            {
                ctrller.timer.StopTimer();
                ctrller._animator.speed = 0.0f;
            }
        else
            foreach (AIController ctrller in aIControllers)
            {
                if (ctrller.waiting) 
                    ctrller.timer.StartTimer();
                ctrller._animator.speed = 1.0f;
            }
    }

    public void ResetAIs()
    {
        foreach (AIController ctrller in aIControllers)
            ctrller.ResetAI();
    }
}
