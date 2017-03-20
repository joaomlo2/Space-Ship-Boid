using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    public float Speed = 3.0f;
    public float rotationSpeed = 2.0f;
    public Vector3 averageDirection;
    public Vector3 averagePosition;
    public float neighbourDistance = 5.0f;


    public Vector3 GoalPosition;
    private float _goalResetTimer = 0;

    private bool turning = false;

    void Start()
    {
        Speed = Random.Range(10.0f, 50.0f);
    }

    void OnDrawGizmos()
    {
        Debug.DrawLine(this.transform.position,GoalPosition,Random.ColorHSV());
    }

    void Update()
    {
        if (GlobalController.instance.Player.GetComponent<PlayerShipController>().FormationModeActive)
        {
            GoalPosition = GlobalController.instance.Player.transform.position;
        }
        else
        {
            Wander();
        }

        //turning = true;
        if (Vector3.Distance(transform.position, Vector3.zero) >= 200)
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
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction),
                rotationSpeed*Time.deltaTime);
            Speed = Random.Range(10.0f, 50.0f);
        }
        else
        {
            if (Random.Range(0, 5) < 1)
                ApplyRules();
        }
        transform.Translate(0, 0, Time.deltaTime*Speed);
    }

    void Wander()
    {
        if (_goalResetTimer >= 5)
        {
            GoalPosition = new Vector3(Random.Range(-300, 300), Random.Range(-50, 50), Random.Range(-200, 200));
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
                    Flock anotherFlock = ships[i].GetComponent<Flock>();
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