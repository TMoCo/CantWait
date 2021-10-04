using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsRagdoll : MonoBehaviour
{
    public Transform headRef;
    public Transform lowerLeftArmRef;
    public Transform lowerLeftLegRef;
    public Transform lowerRightArmRef;
    public Transform lowerRightLegRef;
    public Transform lowerTorsoRef;
    public Transform middleTorsoRef;
    public Transform neckRef;
    public Transform upperLeftArmRef;
    public Transform upperLeftLegRef;
    public Transform upperRightArmRef;
    public Transform upperRightLegRef;
    public Transform upperTorsoRef;

    public int numMeshes = 13;
    private Transform[] targetTransforms;
    private Transform[] sourceTransforms;
    // Start is called before the first frame update
    void Start()
    {
        targetTransforms = new Transform[] {headRef,
                                            lowerLeftArmRef,
                                            lowerLeftLegRef,
                                            lowerRightArmRef,
                                            lowerRightLegRef,
                                            lowerTorsoRef,
                                            middleTorsoRef,
                                            neckRef,
                                            upperLeftArmRef,
                                            upperLeftLegRef,
                                            upperRightArmRef,
                                            upperRightLegRef,
                                            upperTorsoRef};

        sourceTransforms = new Transform[] {this.gameObject.transform.GetChild(0),
                                            this.gameObject.transform.GetChild(1),
                                            this.gameObject.transform.GetChild(2),
                                            this.gameObject.transform.GetChild(3),
                                            this.gameObject.transform.GetChild(4),
                                            this.gameObject.transform.GetChild(5),
                                            this.gameObject.transform.GetChild(6),
                                            this.gameObject.transform.GetChild(7),
                                            this.gameObject.transform.GetChild(8),
                                            this.gameObject.transform.GetChild(9),
                                            this.gameObject.transform.GetChild(10),
                                            this.gameObject.transform.GetChild(11),
                                            this.gameObject.transform.GetChild(12)};
    }

    void MatchPos(Transform source, Transform target)
    {
        Vector3 tPos = target.position;
        source.position = new Vector3(tPos.x + 1.0f, tPos.y, tPos.z);
        source.rotation = target.rotation;
        source.localScale = new Vector3(100.0f, 100.0f, 100.0f);
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < numMeshes; i++)
        {
            MatchPos(sourceTransforms[i], targetTransforms[i]);
        }
    }
}
