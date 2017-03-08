using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

public class AllyShipController : MonoBehaviour
{
    public float Speed;
    public float Acceleration;
    public float TurningSpeed;

    public GameObject CurrentDestination;

    void Awake()
    {
        Speed = 0.0f;
        Acceleration = 0.0f;
        SeekDestination();
    }

    void Update()
    {
        AINavigation();
        Speed += 0.1f * Acceleration;
    }

    void SeekDestination()
    {
        //Look for a place in the player-led formation should the player want to make one
        //Needs new condition to do this in case the player is nearby or perhaps the AllyFleetManager could handle it instead.
        if (GlobalController.instance.Player.GetComponent<PlayerShipController>().FormationModeActive)
        {
            for (int i = 0; i < GlobalController.instance.Player.transform.childCount; i++)
            {
                if (GlobalController.instance.Player.transform.GetChild(i).GetComponent<FormationPointController>().IsClear())
                {
                    CurrentDestination = GlobalController.instance.Player.transform.GetChild(i).gameObject;
                }
            }
        }
        //Or else, it will wander off and attack enemy ships
        CurrentDestination = null;
    }

    void AINavigation()
    {
        //Direction Managment
        Vector3 desiredForwardVector = CurrentDestination.transform.position - transform.position;
        transform.forward = Vector3.Lerp(transform.forward, desiredForwardVector, 1);
        //Speed Managment
        float maxSpeed = 10;
        Speed += 0.1f*Acceleration;
    }

    void Accelerate()
    {
        Acceleration += 0.1f * Time.deltaTime;
    }

    void Decelerate()
    {
        Acceleration -= 0.1f * Time.deltaTime;
    }
}
