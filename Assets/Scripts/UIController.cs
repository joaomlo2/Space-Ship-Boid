using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private GameObject _playerShip;
    public GameObject AccelerationMeter;
    public GameObject SpeedMeter;

    void Awake()
    {
        AccelerationMeter = transform.FindChild("AccelerationIndicator").gameObject;
        SpeedMeter = transform.FindChild("SpeedIndicator").gameObject;
        _playerShip = GameObject.Find("Player");
    }

    void Update()
    {
        AccelerationMeter.GetComponent<Text>().text = "Acceleration: " +
                                                      _playerShip.GetComponent<PlayerShipController>().Acceleration;
        SpeedMeter.GetComponent<Text>().text = "Speed: " +
                                                      _playerShip.GetComponent<PlayerShipController>().Speed;
    }
}
