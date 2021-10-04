using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class cameraSpline : MonoBehaviour
{
    public Vector3 aPos;
    public Vector3 bPos;
    public Vector3 aRot;
    public Vector3 bRot;
    public float duration;
    private float progress;


    // Start is called before the first frame update
    void Start()
    {
        progress = 0.0f;
        transform.position = aPos;
    }

    // Update is called once per frame
    void Update()
    {
        progress += Time.deltaTime;

        float p = Math.Min(progress / duration, 1.0f);

        Vector3 toTarget = bPos - aPos;
        transform.position = aPos + toTarget * p;

        transform.rotation = Quaternion.Lerp(Quaternion.Euler(aRot), Quaternion.Euler(bRot), p);

    }
}
