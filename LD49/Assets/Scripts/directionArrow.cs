using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class directionArrow : MonoBehaviour
{
    GameObject levelManager;
    public Transform waiter;
    float x;
    // Start is called before the first frame update
    void Start()
    {
        levelManager = GameObject.Find("LevelManager");
        x = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        x += 1.0f;
        Vector3 target = levelManager.GetComponent<BoxCollider>().center;
        Vector3 toTarget = (target - waiter.position);

        transform.rotation = Quaternion.LookRotation(toTarget, Vector3.up);
        Vector3 rot = transform.rotation.eulerAngles;
        transform.rotation =  Quaternion.Euler(rot.x, rot.y, x);
        transform.position = new Vector3(waiter.position.x, 5.0f, waiter.position.z);
    }
}
