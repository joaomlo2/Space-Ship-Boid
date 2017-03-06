using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerShipController : MonoBehaviour
{
    public float Speed;
    public float TurningStrength;

    public float DebugLineLength = 3.0f;

	void Update ()
    {
        ProcessMovement();
	}

    void ProcessMovement()
    {
        Vector3 movementToApply=Vector3.zero;
        //Speed
        if (Input.GetKey(KeyCode.A))
        {
            Speed += 0.1f*Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.Z))
        {
            Speed -= 0.1f * Time.deltaTime;
            if (Speed < 0)
                Speed = 0;
        }
        //Rotation
        if (Input.GetKey(KeyCode.I))
        {
            transform.Rotate(TurningStrength, 0.0f,0.0f);
        }
        if (Input.GetKey(KeyCode.K))
        {
            transform.Rotate(-TurningStrength, 0.0f, 0.0f);
        }
        if (Input.GetKey(KeyCode.J))
        {
            transform.Rotate(0.0f, -TurningStrength, 0.0f);
        }
        if (Input.GetKey(KeyCode.L))
        {
            transform.Rotate(0.0f, TurningStrength, 0.0f);
        }
        if (Input.GetKey(KeyCode.U))
        {
            transform.Rotate(0.0f, 0.0f, TurningStrength);
        }
        if (Input.GetKey(KeyCode.O))
        {
            transform.Rotate(0.0f, 0.0f, -TurningStrength);
        }
        transform.position += transform.forward*Speed;
    }

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
