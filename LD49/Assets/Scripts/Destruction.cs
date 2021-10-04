using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destruction : MonoBehaviour
{
    private bool hit = false;
    private Vector3 startPos;
    GameObject levelManager;
    public string objectType;


    // Start is called before the first frame update
    void Start()
    {
         levelManager = GameObject.Find("LevelManager");
         startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(!hit)
        {
            if((transform.position - startPos).magnitude > 1.7f)
            {
                hit = true;
                levelManager.GetComponent<LevelManager>().Destroyed(transform.position, objectType);
            }
        }
    }
}
