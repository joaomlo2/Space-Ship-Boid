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

	//Obstacle Avoidance Testing Presets
	public Vector3 GoalPosition;
	public GameObject PointBeingPursued=null;
	//Obstacle Avoidance
	private int _chosenPathIndex;
	private bool isAvoidingObstacle=false;
	public Vector3 PriorityGoalPosition = Vector3.zero;
	private bool _candidatesInitialized = false;
	private Ray _desiredPath;
	private bool _obstacleDetected = false;
	private float _candidateRotationIncrement=20f;
	private float _candidateRotationIncrementCompensation=50f;
	private CandidateDirection[] _candidateDirections;
	private float[] _candidatePointsDistanceToMainGoal;
	private float _lineOfSightSize;

	class CandidateDirection{
		public GameObject directionHolder;
		public bool isValid;
		public int OppositeIndex;
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
		GoalPosition = PointBeingPursued.transform.position;

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
		//_DrawCandidates ();
		//Debug.DrawLine (transform.position,transform.position+((GoalPosition - transform.position).normalized)*_lineOfSightSize,Color.green);
		//if(isAvoidingObstacle){
		//	Debug.DrawLine(transform.position,PriorityGoalPosition,Color.red);
		//}
    }

    void _DrawForwardVector()
    {
		Debug.DrawLine (transform.position, transform.position + (transform.forward * _lineOfSightSize),Color.magenta);
    }


	//Obstacle Detection Stuff

	private void _InitializeAuxiliaryAvoidanceStructures()
	{
		_candidateDirections = new CandidateDirection[4];
		_candidatePointsDistanceToMainGoal=new float[4];
		_chosenPathIndex = -1;
		for (int i = 0; i < _candidateDirections.Length; i++) {
			_candidateDirections [i] = new CandidateDirection {
				directionHolder = new GameObject(),
				isValid = false
			};
			_candidateDirections [i].directionHolder.transform.parent = transform;
			_candidateDirections [i].directionHolder.transform.forward = transform.forward;
			_candidateDirections [i].directionHolder.transform.up = transform.up;
			_candidateDirections [i].directionHolder.transform.right = transform.right;
			_candidatePointsDistanceToMainGoal[i] = -1;
			switch (i) {
			case 0:
				_candidateDirections [i].OppositeIndex = 1;
				break;
			case 1:
				_candidateDirections [i].OppositeIndex = 0;
				break;
			case 2:
				_candidateDirections [i].OppositeIndex = 3;
				break;
			case 3:
				_candidateDirections [i].OppositeIndex = 2;
				break;
			}
		}
		_candidatesInitialized = true;
	}

	private void _CheckForObstacles(){
		RaycastHit hit;
		_lineOfSightSize = 20+(2 * Speed);
		//_desiredPath=new Ray(transform.position,transform.position+((GoalPosition-transform.position).normalized*_lineOfSightSize));
		_desiredPath = new Ray (transform.position, transform.position + (transform.forward * _lineOfSightSize));
		_candidateRotationIncrement = 10 * (Speed*5);
		if (Physics.Raycast (_desiredPath, out hit, _lineOfSightSize)) {
			if (hit.collider.gameObject != PointBeingPursued) {
				if (!isAvoidingObstacle) {
					if (_ThereIsAWayAround ()) {
						_ChoosePathToTake ();
						isAvoidingObstacle = true;
					} else {
						_UpdateAuxiliaryAvoidanceStructures (_candidateRotationIncrement);
					}
				}
			}
		} else {
			PriorityGoalPosition = Vector3.zero;
			if (isAvoidingObstacle) {
				isAvoidingObstacle = false;
			}
			_InitializeAuxiliaryAvoidanceStructures ();
		}
		if (isAvoidingObstacle) {
			Ray oppositeCandidateRay = new Ray (transform.position, _candidateDirections [_candidateDirections [_chosenPathIndex].OppositeIndex].directionHolder.transform.forward);
			RaycastHit oppositeCandidateHit;
			if (Physics.Raycast (oppositeCandidateRay, out oppositeCandidateHit, (_lineOfSightSize + _candidateRotationIncrementCompensation)))
				_TurnShip ();
		}
	}

	private void _DrawCandidates(){
		if (_candidatesInitialized) {
			Debug.DrawLine (transform.position, transform.position + (_candidateDirections [0].directionHolder.transform.forward * _lineOfSightSize), Color.white);
			Debug.DrawLine (transform.position, transform.position + (_candidateDirections [1].directionHolder.transform.forward * _lineOfSightSize), Color.blue);
			Debug.DrawLine (transform.position, transform.position + (_candidateDirections [2].directionHolder.transform.forward * _lineOfSightSize), Color.yellow);
			Debug.DrawLine (transform.position, transform.position + (_candidateDirections [3].directionHolder.transform.forward * _lineOfSightSize), Color.cyan);
		}
	}

	private void _UpdateAuxiliaryAvoidanceStructures(float valueToIncrement)
	{
		_candidateDirections [0].directionHolder.transform.Rotate (-valueToIncrement * Time.deltaTime, 0, 0);
		_candidateDirections [1].directionHolder.transform.Rotate (valueToIncrement * Time.deltaTime, 0, 0);
		_candidateDirections [2].directionHolder.transform.Rotate (0, valueToIncrement * Time.deltaTime, 0);
		_candidateDirections [3].directionHolder.transform.Rotate (0, -valueToIncrement * Time.deltaTime, 0);
		_candidateRotationIncrementCompensation += 5;
	}
	private bool _ThereIsAWayAround()
	{
		for (int i = 0; i < _candidateDirections.Length; i++) {
			Ray r = new Ray (transform.position, _candidateDirections [i].directionHolder.transform.forward);
			RaycastHit auxHit;
			if (Physics.Raycast (r, out auxHit, _lineOfSightSize+_candidateRotationIncrementCompensation)) {
				_candidateDirections [i].isValid = false;
			} else {
				_candidateDirections [i].isValid = true;
				_candidatePointsDistanceToMainGoal [i] = Vector3.Distance (transform.position + (_candidateDirections [i].directionHolder.transform.forward * _lineOfSightSize), GoalPosition);
				return true;
			}
		}
		return false;
	}

	//Método do forçar de viragem

	private void _ChoosePathToTake()
	{
		float shortestDistance = float.MaxValue;
		for (int i = 0; i < _candidateDirections.Length; i++) {
			if (_candidateDirections [i].isValid && _candidatePointsDistanceToMainGoal [i] < shortestDistance) {
				_chosenPathIndex = i;
				shortestDistance = _candidatePointsDistanceToMainGoal [i];
			}
		}
	}

	private void _TurnShip(){
		if (_chosenPathIndex != -1) {
			switch (_chosenPathIndex) {
			case 0:
				Debug.Log ("Chosen UP path");
				transform.Rotate(-TurningSpeed*10*Time.deltaTime, 0.0f, 0.0f);
				break;
			case 1:
				Debug.Log ("Chosen DOWN path");
				transform.Rotate(TurningSpeed*10*Time.deltaTime, 0.0f, 0.0f);
				break;
			case 2:
				Debug.Log ("Chosen RIGHT path");
				transform.Rotate(0.0f, TurningSpeed*10*Time.deltaTime, 0.0f);
				break;
			case 3:
				Debug.Log ("Chosen LEFT path");
				transform.Rotate(0.0f, -TurningSpeed*10*Time.deltaTime, 0.0f);
				break;
			}
		}
	}

//	//Método do cálculo do novo destino
//	private Vector3 _ChooseWayAround()
//	{
//		float shortestDistance = float.MaxValue;
//		int chosenPath = -1;
//		for (int i = 0; i < _candidateDirections.Length; i++) {
//			if (_candidateDirections [i].isValid && _candidatePointsDistanceToMainGoal [i] < shortestDistance) {
//				chosenPath = i;
//				shortestDistance = _candidatePointsDistanceToMainGoal [i];
//			}
//		}
//		if (chosenPath != -1) {
//			switch (chosenPath) {
//			case 0:
//				Debug.Log ("Chosen UP path");
//				break;
//			case 1:
//				Debug.Log ("Chosen DOWN path");
//				break;
//			case 2:
//				Debug.Log ("Chosen RIGHT path");
//				break;
//			case 3:
//				Debug.Log ("Chosen LEFT path");
//				break;
//			}
//			_UpdateAuxiliaryAvoidanceStructures (_candidateRotationIncrement * 10);
//			return _candidateDirections[chosenPath].directionHolder.transform.forward * 10;
//		}
//		else {
//			return Vector3.zero;
//		}
//	}
//
//	private void CheckIfPriorityPointWasReached()
//	{
//		if (Vector3.Distance (transform.position, PriorityGoalPosition) <= 2)
//			isAvoidingObstacle = false;
//	}
}
