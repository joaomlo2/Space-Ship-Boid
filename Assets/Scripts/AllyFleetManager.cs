using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyFleetManager : MonoBehaviour
{
    public int NumberOfAllies;

    void Awake()
    {
        for (int i = 0; i < NumberOfAllies; i++)
        {
            GameObject newAllyShip = Resources.Load("Prefabs/Ally") as GameObject;
            newAllyShip.name = "Ally " + (i + 1);
            newAllyShip.transform.position = Vector3.zero;
        }
    }

	void Start () {

    }

	void Update () {
	}
}
