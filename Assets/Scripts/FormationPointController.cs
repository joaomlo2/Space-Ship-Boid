using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationPointController : MonoBehaviour
{
    public GameObject currentOccupyingShip;

    void Update()
    {
        if (GlobalController.instance.Player.GetComponent<PlayerShipController>().IsRotating)
        {
            transform.localRotation = GlobalController.instance.Player.transform.rotation;
        }
        else
        {
            transform.localRotation = Quaternion.SlerpUnclamped(transform.localRotation, Quaternion.identity, 0.1f);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            transform.localRotation=Quaternion.identity;
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
