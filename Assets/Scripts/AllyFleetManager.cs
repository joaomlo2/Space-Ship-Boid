using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
            allyArray[i].transform.position = new Vector3(Random.Range(transform.position.x-100, transform.position.x+100), Random.Range(transform.position.y - 50, transform.position.y+50), Random.Range(transform.position.z - 100, transform.position.z+100));
            allyArray[i].transform.SetParent(transform);
        }
    }

    void Update()
    {
        if (GlobalController.instance.Player.GetComponent<PlayerShipController>().FormationModeActive)
        {
            HandleFormation();
        }
    }

    void HandleFormation()
    {
        for (int i = 0; i < GlobalController.instance.Player.transform.FindChild("FormationPoints").childCount; i++)
        {
            if (
                GlobalController.instance.Player.transform.FindChild("FormationPoints")
                    .transform.GetChild(i)
                    .GetComponent<FormationPointController>()
                    .IsClear())
            {
                GlobalController.instance.Player.transform.FindChild("FormationPoints")
                        .transform.GetChild(i)
                        .GetComponent<FormationPointController>().currentOccupyingShip =
                    allyArray[ShipClosestToPlayer()];
                GlobalController.instance.Player.transform.FindChild("FormationPoints")
                        .transform.GetChild(i)
                        .GetComponent<FormationPointController>().currentOccupyingShip.GetComponent<AIShipController>().belongsToFormation
                    = true;
                GlobalController.instance.Player.transform.FindChild("FormationPoints")
                        .transform.GetChild(i)
                        .GetComponent<FormationPointController>()
                        .currentOccupyingShip.GetComponent<AIShipController>()
                        .PointBeingPursued =
                    GlobalController.instance.Player.transform.FindChild("FormationPoints")
                        .transform.GetChild(i).gameObject;
            }
        }
    }

    public void ClearFormation()
    {
        for (int i = 0; i < GlobalController.instance.Player.transform.FindChild("FormationPoints").childCount; i++)
        {
            GlobalController.instance.Player.transform.FindChild("FormationPoints")
                .GetChild(i)
                .GetComponent<FormationPointController>()
                .currentOccupyingShip.GetComponent<AIShipController>().belongsToFormation = false;
            GlobalController.instance.Player.transform.FindChild("FormationPoints")
                .GetChild(i)
                .GetComponent<FormationPointController>()
                .currentOccupyingShip.GetComponent<AIShipController>().PointBeingPursued = null;
            GlobalController.instance.Player.transform.FindChild("FormationPoints")
                .GetChild(i)
                .GetComponent<FormationPointController>()
                .currentOccupyingShip = null;
        }
    }

    int ShipClosestToPlayer()
    {
        float minDist = float.MaxValue;
        int closestShipIndex = 0;
        for (int i = 0; i < allyArray.Length; i++)
        {
            if (Vector3.Distance(allyArray[i].transform.position, GlobalController.instance.Player.transform.position) < minDist && !allyArray[i].GetComponent<AIShipController>().belongsToFormation)
            {
                closestShipIndex = i;
                minDist = Vector3.Distance(allyArray[i].transform.position, GlobalController.instance.Player.transform.position);
            }
        }
        return closestShipIndex;
    }
}
