using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyFleetManager : MonoBehaviour
{
    public int NumberOfAllies;
    public static GameObject[] allyArray;

    void Awake()
    {
        allyArray = new GameObject[NumberOfAllies];
    }

    void Start()
    {
        for (int i = 0; i < NumberOfAllies; i++)
        {
            allyArray[i] = Instantiate(Resources.Load("Prefabs/Ally")) as GameObject;
            allyArray[i].name = "Ally " + (i + 1);
            allyArray[i].transform.position = new Vector3(Random.Range(-100, 100), Random.Range(-50, 50), Random.Range(-100, 100));
            allyArray[i].transform.SetParent(transform);
        }
    }

}
