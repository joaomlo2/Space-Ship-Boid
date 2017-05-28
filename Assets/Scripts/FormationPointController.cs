using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationPointController : MonoBehaviour
{
    public GameObject currentOccupyingShip;

    private Quaternion _previousPlayerShipRotation;
    private Quaternion  targetRot;

    void Update()
    {
        if (GlobalController.instance.Player.transform.rotation != _previousPlayerShipRotation)
        {
            transform.localRotation = GlobalController.instance.Player.transform.rotation;
            _previousPlayerShipRotation = GlobalController.instance.Player.transform.rotation;
        }
        else
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.identity, 0.1f);
        }
    }

    public bool IsClear()
    {
        return currentOccupyingShip == null;
    }

    public void ShipHasLeftFormation()
    {
        currentOccupyingShip = null;
    }
}
