using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerShipController : MonoBehaviour
{
    public float Speed;
    public float Acceleration;
    public float TurningSpeed;
    public float TurningAcceleration;

    public bool FormationModeActive;
    public int NumberOfFreeSpacesInFormation;
    public int MaxFormationSize = 4;
    private int previouslySelectedFormation;
    private int SelectedFormation=0;
    public string[] Formations = {"Line", "Wall", "Arrow"};

    //Future Implementation
    private short _formationType;

    public float DebugLineLength = 3.0f;

    void Awake()
    {
        FormationModeActive = false;
        previouslySelectedFormation = SelectedFormation;
    }

	void Update ()
    {
        ProcessMovement();
        FormationController();

    }

    void FormationController()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!FormationModeActive)
            {
                FormationModeActive = !FormationModeActive;
            }
            else
            {
                GlobalController.instance.AlliesHolder.GetComponent<AllyFleetManager>().ClearFormation();
                FormationModeActive = !FormationModeActive;
            }
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (SelectedFormation == Formations.Length-1)
            {
                SelectedFormation = 0;
            }
            else
            {
                SelectedFormation++;
            }
        }

        //if (SelectedFormation != previouslySelectedFormation)
        //{
        //    foreach (Transform t in transform.FindChild("FormationPoints"))
        //    {
        //        Destroy(t.gameObject);
        //    }
        //    switch (SelectedFormation)
        //    {
        //        case 0:
        //            for (int i = 0; i < MaxFormationSize; i++)
        //            {
        //                float spacingBetweenFormationPoints = 10;
        //                GameObject point=Instantiate(Resources.Load("Prefabs/FormationPoint")) as GameObject;
        //                point.transform.position = (transform.right*spacingBetweenFormationPoints)* (i + 1);
        //                point.transform.forward = transform.forward;
        //                point.transform.parent = transform.FindChild("FormationPoints");
        //            }
        //            break;
        //        case 1:
        //            for (int i = 0; i < MaxFormationSize; i++)
        //            {
        //                float spacingBetweenFormationPoints = 5;
        //                float heightDifference = 2;
        //                GameObject point = Instantiate(Resources.Load("Prefabs/FormationPoint")) as GameObject;
        //                point.transform.position = (transform.right * spacingBetweenFormationPoints) * (i + 1);
        //                point.transform.parent = transform.FindChild("FormationPoints");
        //                if (i%2 != 0)
        //                {
        //                    point.transform.position=new Vector3(point.transform.position.x, point.transform.position.y-heightDifference, point.transform.position.z);
        //                }
        //                point.transform.forward = transform.forward;
        //            }
        //            break;
        //        case 2:
        //            for (int i = 0; i < MaxFormationSize; i++)
        //            {
        //                float spacingBetweenFormationPoints = 10;
        //                GameObject point = Instantiate(Resources.Load("Prefabs/FormationPoint")) as GameObject;
        //                point.transform.position = (Vector3.Cross(transform.right,-transform.forward) * spacingBetweenFormationPoints) * (i+1);
        //                point.transform.forward = transform.forward;
        //                point.transform.parent = transform.FindChild("FormationPoints");
        //            }
        //            break;
        //        default:
        //            break;
        //    }
        //    previouslySelectedFormation = SelectedFormation;
        //}
    }

    //Movement Related Stuff
    void ProcessMovement()
    {
        Vector3 AccelerationVector;
        //Speed
        if (Input.GetKey(KeyCode.A))
        {
            Acceleration += 0.1f*Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.Z))
        {
            Acceleration -= 0.1f * Time.deltaTime;
        }
        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.Z))
        {
            Acceleration = 0;
        }
        Speed += 0.1f * Acceleration;
        //Rotation
        if (Input.GetKey(KeyCode.I))
        {
            transform.Rotate(TurningSpeed, 0.0f,0.0f);
        }
        if (Input.GetKey(KeyCode.K))
        {
            transform.Rotate(-TurningSpeed, 0.0f, 0.0f);
        }
        if (Input.GetKey(KeyCode.J))
        {
            transform.Rotate(0.0f, -TurningSpeed, 0.0f);
        }
        if (Input.GetKey(KeyCode.L))
        {
            transform.Rotate(0.0f, TurningSpeed, 0.0f);
        }
        if (Input.GetKey(KeyCode.U))
        {
            transform.Rotate(0.0f, 0.0f, TurningSpeed);
        }
        if (Input.GetKey(KeyCode.O))
        {
            transform.Rotate(0.0f, 0.0f, -TurningSpeed);
        }
        transform.position += transform.forward * Speed;
    }

    //Gizmos Stuff
    void OnDrawGizmos()
    {
        DrawForwardVector();
    }

    void DrawForwardVector()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, transform.position + (DebugLineLength * transform.forward));
    }
}
