using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    public float Speed = 3.0f;
    public float rotationSpeed = 4.0f;
    public Vector3 averageDirection;
    public Vector3 averagePosition;
    public float neighbourDistance = 5.0f;

    void Start()
    {
        Speed = Random.Range(10.0f, 50.0f);
    }

    void Update()
    {
        if (Random.Range(0, 5) < 1)
            ApplyRules();
        transform.Translate(0, 0, Time.deltaTime*Speed);
    }

    void ApplyRules()
    {
        GameObject[] ships = AllyFleetManager.allyArray;
        Vector3 vCentre = Vector3.zero;
        Vector3 vAvoid = Vector3.zero;
        float gSpeed = 0.1f;
        Vector3 goalPos = AllyFleetManager.GoalPosition;
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
                    if (dist < 1f)
                        vAvoid += this.transform.position - ships[i].transform.position;

                    Flock anotherFlock = ships[i].GetComponent<Flock>();
                    gSpeed += anotherFlock.Speed;
                }
            }
        }
        if (groupSize > 0)
        {
            vCentre = vCentre/groupSize + (goalPos - this.transform.position);
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