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
    public float MaxSpeed = 50.0f;
    public float Speed = 3.0f;
    public float rotationSpeed = 0.1f;// inicial é 3.0f
    private Vector3 averageDirection;
    private Vector3 averagePosition;
    private float neighbourDistance = 5.0f;

    public int Faction;//1-Player Faction 2-Enemy Faction

    //public int Health = 100;

    public bool isInPosition;
    private float _slowingDownRadius = 5f;
    public bool belongsToFormation = false;
    public GameObject PointBeingPursued;
    private Vector3 GoalPosition;
    private float _goalResetTimer = 0;

    private bool turning = false;

	private float TurningSpeed=2f;
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

    void Start()
    {
        currentState=ShipState.Wandering;
		Speed = UnityEngine.Random.Range(10.0f, 50.0f);
		_InitializeAuxiliaryAvoidanceStructures ();
    }

    void OnDrawGizmos()
    {
		DrawLineOfSight ();
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
		CheckForObstacles ();
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
                Evade();
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
                Speed = UnityEngine.Random.Range(10.0f, MaxSpeed);
            }
            else
            {

                //O FORMATION POINT É QUE VAI TER DE FICAR ENCARREGADO DISTO

                //transform.rotation = Quaternion.Slerp(transform.rotation,
                //    Quaternion.LookRotation(PointBeingPursued.transform.forward,
                //        PointBeingPursued.transform.up),
                //    rotationSpeed * Time.deltaTime);
                //transform.rotation = Quaternion.Slerp(transform.rotation, PointBeingPursued.transform.rotation, rotationSpeed * Time.deltaTime);
                //Speed = GlobalController.instance.Player.GetComponent<PlayerShipController>().Speed-0.1f;
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

    void Evade()
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
        isInPosition = Vector3.Distance(transform.position, PointBeingPursued.transform.position) <= _slowingDownRadius;
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

    void ShouldIStayOrShouldIGo()
    {
        int closestShipindex = -1;
        float minDistanceFound = float.MaxValue;
        switch (Faction)
        {
            case 1:
                for (int i = 0; i < GlobalController.instance.EnemiesHolder.transform.childCount; i++)
                {
                    if (
                        Vector3.Distance(transform.position,
                            GlobalController.instance.EnemiesHolder.transform.GetChild(i).position) > minDistanceFound)
                    {
                        minDistanceFound = Vector3.Distance(transform.position,
                            GlobalController.instance.EnemiesHolder.transform.GetChild(i).position);
                        closestShipindex = i;
                    }
                }
                PointBeingPursued =
                    GlobalController.instance.EnemiesHolder.transform.GetChild(closestShipindex).gameObject;
                break;
            case 2:
                for (int i = 0; i < GlobalController.instance.AlliesHolder.transform.childCount; i++)
                {
                    if (
                        Vector3.Distance(transform.position,
                            GlobalController.instance.AlliesHolder.transform.GetChild(i).position) > minDistanceFound)
                    {
                        minDistanceFound = Vector3.Distance(transform.position,
                            GlobalController.instance.AlliesHolder.transform.GetChild(i).position);
                        closestShipindex = i;
                    }
                }
                PointBeingPursued =
                    GlobalController.instance.AlliesHolder.transform.GetChild(closestShipindex).gameObject;
                break;
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

	void DrawLineOfSight(){
		Debug.DrawLine (transform.position, transform.position + (transform.forward * _lineOfSightSize),Color.magenta);
	}

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
}