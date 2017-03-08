using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyFleetManager : MonoBehaviour
{
    public int NumberOfAllies;

    void Awake()
    {
        int counter = 0;
        foreach (Transform FormationSpot in GameObject.Find("Player").transform.FindChild("FormationPoints"))
        {
            GameObject newAllyShip = Instantiate(Resources.Load("Prefabs/Ally")) as GameObject;
            newAllyShip.name = "Ally " + (counter + 1);
            newAllyShip.transform.position = FormationSpot.position;
            newAllyShip.transform.SetParent(transform);
            counter++;
        }
    }

	void Update () {

	}
}
