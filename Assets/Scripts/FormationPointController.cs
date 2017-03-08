using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationPointController : MonoBehaviour
{
    public GameObject currentOccupyingShip;

    public bool IsClear()
    {
        return currentOccupyingShip == null;
    }

    public void ShipHasLeftFormation()
    {
        currentOccupyingShip = null;
    }
}
