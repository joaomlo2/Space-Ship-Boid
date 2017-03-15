using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyFleetManager : MonoBehaviour
{
    public int NumberOfAllies;
    public static Vector3 GoalPosition;
    private float _goalResetTimer = 0;
    public static GameObject[] allyArray;

    void Awake()
    {
        allyArray = new GameObject[NumberOfAllies];
    }

    void Start()
    {
        for (int i = 0; i < NumberOfAllies; i++)
        {
            allyArray[i] = Instantiate(Resources.Load("Prefabs/Ally")) as GameObject;
            allyArray[i].name = "Ally " + (i + 1);
            allyArray[i].transform.position = new Vector3(Random.Range(-100, 100), Random.Range(-50, 50), Random.Range(-100, 100));
            allyArray[i].transform.SetParent(transform);
        }
    }

	void Update () {
        //GoalPosition = Player.transform.position;
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
}
