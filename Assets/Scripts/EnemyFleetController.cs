using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFleetController : MonoBehaviour {

    public int NumberOfEnemies;
    public static GameObject[] enemyArray;

    void Awake()
    {
        enemyArray = new GameObject[NumberOfEnemies];
    }

    void Start()
    {
        for (int i = 0; i < NumberOfEnemies; i++)
        {
            enemyArray[i] = Instantiate(Resources.Load("Prefabs/Enemy")) as GameObject;
            enemyArray[i].name = "Enemy " + (i + 1);
            enemyArray[i].transform.position = new Vector3(Random.Range(-100, 100), Random.Range(-50, 50), Random.Range(-100, 100));
            enemyArray[i].transform.SetParent(transform);
        }
    }

    void Update()
    {

    }
}
