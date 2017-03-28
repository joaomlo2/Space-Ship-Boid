using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIShipController : MonoBehaviour
{
    public enum ShipState
    {
        Wandering,
        Engaging,
        Formation,
        Evading
    };

    public ShipState currentState;
    public float Speed = 3.0f;
    public float rotationSpeed = 2.0f;
    private Vector3 averageDirection;
    private Vector3 averagePosition;
    private float neighbourDistance = 5.0f;

    public int Faction;//1-Player Faction 2-Enemy Faction

    public bool isInPosition;
    public bool belongsToFormation = false;
    public GameObject PointBeingPursued;
    private Vector3 GoalPosition;
    private float _goalResetTimer = 0;

    private bool turning = false;

    void Start()
    {
        currentState=ShipState.Wandering;
        Speed = UnityEngine.Random.Range(10.0f, 50.0f);
    }

    void OnDrawGizmos()
    {
        if (Faction == 1)
        {
            if (belongsToFormation)
                Debug.DrawLine(this.transform.position, GoalPosition, Color.green);
            else
            {
                Debug.DrawLine(this.transform.position, GoalPosition, Color.blue);
            }
        }
        else
        {
            Debug.DrawLine(this.transform.position, GoalPosition, Color.red);
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (belongsToFormation)
        {
            if (collider.gameObject == PointBeingPursued)
            {
                isInPosition = true;
            }
        }
    }

    void OnTriggerExit()
    {
        isInPosition = false;
    }

    void Update()
    {
        HandleFactionBasedBehaviour();
        HandleMovement();
    }

    private void HandleFactionBasedBehaviour()
    {
        switch (currentState)
        {
            case ShipState.Formation:
                if (Faction == 1)
                {
                    if (GlobalController.instance.Player.GetComponent<PlayerShipController>().FormationModeActive &&
            belongsToFormation)
                    {
                        CheckIfIsInPosition();
                        GoalPosition = PointBeingPursued.transform.position;
                    }
                }
                break;
            case ShipState.Engaging:
                Pursue();
                break;
            case ShipState.Evading:
                Flee();
                break;
            case ShipState.Wandering:
                Wander();
                break;
        }
    }

    private void HandleMovement()
    {
        if (Vector3.Distance(transform.position, Vector3.zero) >= 200 || belongsToFormation)
        {
            turning = true;
        }
        else
        {
            turning = false;
        }
        if (turning)
        {
            Vector3 direction = GoalPosition - transform.position;
            if (!isInPosition)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction),
                    rotationSpeed * Time.deltaTime);
                if (!belongsToFormation)
                    Speed = UnityEngine.Random.Range(10.0f, 50.0f);
                else
                {
                    Speed = UnityEngine.Random.Range(10.0f, 50.0f);
                }
            }
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    Quaternion.LookRotation(PointBeingPursued.transform.forward,
                        PointBeingPursued.transform.up),
                    rotationSpeed * Time.deltaTime);
                Speed = GlobalController.instance.Player.GetComponent<PlayerShipController>().Speed;
            }
            ApplyRules();
        }
        else
        {
            if (UnityEngine.Random.Range(0, 5) < 1)
                ApplyRules();
        }
        transform.Translate(0, 0, Time.deltaTime * Speed);
    }

    void Flee()
    {
        if (_goalResetTimer >= 5)
        {
            GoalPosition = new Vector3(UnityEngine.Random.Range(-200, 200), UnityEngine.Random.Range(-200, 200), UnityEngine.Random.Range(-200, 200));
            _goalResetTimer = 0;
        }
        else
        {
            _goalResetTimer += Time.deltaTime;
        }
    }

    void Pursue()
    {
    }

    void CheckIfIsInPosition()
    {
        isInPosition = Vector3.Distance(transform.position, PointBeingPursued.transform.position) <= 6f;
    }

    void Wander()
    {
        if (_goalResetTimer >= 5)
        {
            GoalPosition = new Vector3(UnityEngine.Random.Range(-200, 200), UnityEngine.Random.Range(-200, 200), UnityEngine.Random.Range(-200, 200));
            _goalResetTimer = 0;
        }
        else
        {
            _goalResetTimer += Time.deltaTime;
        }
    }

    void ApplyRules()
    {
        GameObject[] ships = AllyFleetManager.allyArray;
        Vector3 vCentre = Vector3.zero;
        Vector3 vAvoid = Vector3.zero;
        float gSpeed = 0.1f;

        float dist;

        int groupSize = 0;
        for (int i = 0; i < ships.Length; i++)
        {
            if (ships[i] != this.gameObject)
            {
                dist = Vector3.Distance(ships[i].transform.position, this.transform.position);
                if (dist <= neighbourDistance)
                {
                    vCentre += ships[i].transform.position;
                    groupSize++;
                    if (dist < 1.0f)
                    {
                        vAvoid += this.transform.position - ships[i].transform.position;
                    }
                    AIShipController anotherFlock = ships[i].GetComponent<AIShipController>();
                    gSpeed += anotherFlock.Speed;
                }
            }
        }
        if (groupSize > 0)
        {
            vCentre = vCentre/groupSize + (GoalPosition - this.transform.position);
            Speed = gSpeed/groupSize;
            Vector3 direction = (vCentre + vAvoid) - transform.position;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction),
                    rotationSpeed*Time.deltaTime);
            }
        }
    }
}