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
		if (NumberOfEnemies > 0) {
			for (int i = 0; i < NumberOfEnemies; i++) {
				enemyArray [i] = Instantiate (Resources.Load ("Prefabs/Enemy")) as GameObject;
				enemyArray [i].name = "Enemy " + (i + 1);
				enemyArray [i].transform.position = new Vector3 (Random.Range (transform.position.x - 100, transform.position.x + 100), Random.Range (transform.position.y - 50, transform.position.y + 50), Random.Range (transform.position.z - 100, transform.position.z + 100));
				enemyArray [i].transform.SetParent (transform);
			}
		}
    }
}
