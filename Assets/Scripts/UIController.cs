using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameObject AccelerationMeter;
    public GameObject SpeedMeter;
    public GameObject FormationIndicator;

    void Awake()
    {
        AccelerationMeter = transform.FindChild("AccelerationIndicator").gameObject;
        SpeedMeter = transform.FindChild("SpeedIndicator").gameObject;
        FormationIndicator = transform.FindChild("FormationIndicator").gameObject;
    }

    void Update()
    {
        AccelerationMeter.GetComponent<Text>().text = "Acceleration: " +
                                                      GlobalController.instance.Player.GetComponent<PlayerShipController>().Acceleration;
        SpeedMeter.GetComponent<Text>().text = "Speed: " +
                                                      GlobalController.instance.Player.GetComponent<PlayerShipController>().Speed;
        FormationIndicator.GetComponent<Text>().text="Formation is On: " +
                                                      GlobalController.instance.Player.GetComponent<PlayerShipController>().FormationModeActive;
    }
}
