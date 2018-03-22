using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KartUI : MonoBehaviour 
{
    public Text PlaceText;
    public Text TimeText;
    public Text SpeedText;

    void Start()
    {
        PlaceText.text = "1st";
    }

    void Update()
    {
        
    }

    public void SetSpeed(int speed)
    {
        SpeedText.text = speed.ToString() + " MPH";
    }
}
