using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomTable : MonoBehaviour
{
    public GameObject[] randomFood;
    public GameObject[] randomDrink;
    public GameObject[] randomPerson1;
    public GameObject[] randomPerson2;
    public GameObject[] randomPerson3;
    public GameObject[] randomPerson4;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 p = transform.position;
        int seed = (int)(p.x * p.y + p.z * p.x + p.z * p.y + p.x + p.y + p.z);

        Random.InitState(seed);

        // pick a random food / drink
        int food = Random.Range(-1, randomFood.Length);
        if(food != -1)
        {
            randomFood[food].SetActive(true);
        }

        int drink = Random.Range(-1, randomDrink.Length);
        if(drink != -1)
        {
            randomDrink[drink].SetActive(true);
        }

        int p1 = Random.Range(-1, randomPerson1.Length);
        if(p1 != -1)
        {
            randomPerson1[p1].SetActive(true);
        }

        int p2 = Random.Range(-1, randomPerson2.Length);
        if(p2 != -1)
        {
            randomPerson2[p2].SetActive(true);
        }

        int p3 = Random.Range(-1, randomPerson3.Length);
        if(p3 != -1)
        {
            randomPerson3[p3].SetActive(true);
        }

        int p4 = Random.Range(-1, randomPerson4.Length);
        if(p4 != -1)
        {
            randomPerson4[p4].SetActive(true);
        }

        // todo: same for people
    }

    // Update is called once per frame
    void Update()
    {

    }
}
