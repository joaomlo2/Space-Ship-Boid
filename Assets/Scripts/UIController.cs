using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private GameObject _playerShip;
    public GameObject SpeedMeter;

    void Awake()
    {
        SpeedMeter = transform.FindChild("AccelerationIndicator").gameObject;
        _playerShip = GameObject.Find("Player");
    }

    void Update()
    {
        SpeedMeter.GetComponent<Text>().text = "Speed: " +
                                                      _playerShip.GetComponent<PlayerShipController>().Speed;
    }
}
