using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerShipController : MonoBehaviour
{
    public float Speed=3.0f;
    public float Acceleration=0.0f;

    public float DebugLineLength = 3.0f;

	void Update ()
    {

	}

    void ProcessMovement()
    {
        Vector3 movementToApply=Vector3.zero;
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
