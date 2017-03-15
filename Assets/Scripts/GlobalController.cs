using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalController : MonoBehaviour
{
    public static GlobalController instance;
    public GameObject Player;
    public GameObject AlliesHolder;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
        }
        Player=GameObject.Find("Player");
        AlliesHolder=GameObject.Find("Allies");
    }

    void Update()
    {

    }
}
