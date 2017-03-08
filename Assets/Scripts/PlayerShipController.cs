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
    //Future Implementation
    private short _formationType;

    public float DebugLineLength = 3.0f;

    void Awake()
    {
        FormationModeActive = true;
    }

	void Update ()
    {
        ProcessMovement();
	}

    //Movement Related Stuff
    void ProcessMovement()
    {
        //Speed
        if (Input.GetKey(KeyCode.A))
        {
            Acceleration += 0.1f*Time.deltaTime;
        }
        else
        {
            if (Acceleration <= 1)
            {
                Acceleration = 0;
            }
        }
        if (Input.GetKey(KeyCode.Z))
        {
            Acceleration -= 0.1f * Time.deltaTime;
        }
        else
        {
            if (Acceleration >= 1)
            {
                Acceleration = 0;
            }
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
        transform.position += transform.forward*Speed;
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
