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

	//Obstacle Avoidance
	public Vector3 GoalPosition;
	private GameObject PointBeingPursued=null;
	private Vector3 PriorityGoalPosition = Vector3.zero;

	private Ray _desiredPath;
	private bool _obstacleDetected = false;
	private float _candidateRotationIncrement=1f;
	private CandidateDirection[] _candidateDirections;
	private float _lineOfSightSize;

	struct CandidateDirection{
		public Transform direction;
		public bool isValid;
	}

    void Awake()
    {
        FormationModeActive = false;
        previouslySelectedFormation = SelectedFormation;
    }
	void Start()
	{
		_InitializeAuxiliaryAvoidanceStructures ();
	}

	void Update ()
    {
        ProcessMovement();
        FormationController();
		_CheckForObstacles ();
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
        Speed += 1f * Acceleration;
        //Rotation
        if (Input.GetKey(KeyCode.I))
        {
            transform.Rotate(TurningSpeed, 0.0f, 0.0f);
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
        //transform.position += transform.forward * Speed;
        transform.Translate(0, 0, Time.deltaTime * Speed);
    }

    //Gizmos Stuff
    void OnDrawGizmos()
    {
        _DrawForwardVector();
		_DrawCandidates ();
    }

    void _DrawForwardVector()
    {
		Debug.DrawLine (transform.position, transform.position + (transform.forward * _lineOfSightSize),Color.magenta);
    }


	//Obstacle Detection Stuff

	private void _InitializeAuxiliaryAvoidanceStructures()
	{
		_candidateDirections = new CandidateDirection[4];
		for (int i = 0; i < _candidateDirections.Length; i++) {
			_candidateDirections [i] = new CandidateDirection {
				direction = transform,
				isValid = false
			};
		}
	}

	private void _CheckForObstacles(){
		RaycastHit hit;
		_lineOfSightSize = 10+(2 * Speed);
		_desiredPath = new Ray (transform.position, GoalPosition);
		if (Physics.Raycast (_desiredPath, out hit, _lineOfSightSize)) {
			if (hit.collider.gameObject != PointBeingPursued) {
				if (_IsThereAnyWayAround ()) {
					PriorityGoalPosition = _GetWayAround ();
					_InitializeAuxiliaryAvoidanceStructures ();
				} else {
					_UpdateAuxiliaryAvoidanceStructures ();
				}
			}
		} else {
			PriorityGoalPosition = Vector3.zero;
		}
	}

	private void _DrawCandidates(){
		Debug.DrawLine (transform.position, transform.position + (_candidateDirections[0].direction.forward * _lineOfSightSize),Color.white);
		Debug.DrawLine (transform.position, transform.position + (_candidateDirections[1].direction.forward * _lineOfSightSize),Color.blue);
		Debug.DrawLine (transform.position, transform.position + (_candidateDirections[2].direction.forward * _lineOfSightSize),Color.yellow);
		Debug.DrawLine (transform.position, transform.position + (_candidateDirections[3].direction.forward * _lineOfSightSize),Color.cyan);
	}

	private void _UpdateAuxiliaryAvoidanceStructures()
	{
		_candidateDirections [0].direction.Rotate (-_candidateRotationIncrement * Time.deltaTime, 0, 0);
		_candidateDirections [1].direction.Rotate (_candidateRotationIncrement * Time.deltaTime, 0, 0);
		_candidateDirections [2].direction.Rotate (0, _candidateRotationIncrement * Time.deltaTime, 0);
		_candidateDirections [3].direction.Rotate (0, -_candidateRotationIncrement * Time.deltaTime, 0);
	}
	private bool _IsThereAnyWayAround()
	{
		for (int i = 0; i < _candidateDirections.Length; i++) {
			Ray r = new Ray (transform.position, _candidateDirections [i].direction.forward);
			RaycastHit auxHit;
			if (Physics.Raycast (r, out auxHit, _lineOfSightSize)) {
				_candidateDirections [i].isValid = false;
			} else {
				_candidateDirections [i].isValid = true;
				return true;
			}
		}
		return false;
	}

	private Vector3 _GetWayAround()
	{
		for (int i = 0; i < _candidateDirections.Length; i++) {
			if (_candidateDirections [i].isValid)
				return _candidateDirections [i].direction.forward * 150;
		}
		return Vector3.zero;
	}
}
