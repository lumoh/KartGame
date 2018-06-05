using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KartUI : MonoBehaviour 
{
    public Text PlaceText;
    public Text TimeText;
    public Text SpeedText;

    public BetterButton GoBtn;
    public BetterButton FireBtn;
    public Slider WheelSlider;

    void Start()
    {
        PlaceText.text = "1st";

        if (Application.isEditor)
        {
            Util.SetActive(GoBtn, false);
            Util.SetActive(FireBtn, false);
            Util.SetActive(WheelSlider, false);
        }
    }

    void Update()
    {
        
    }

    public void SetSpeed(int speed)
    {
        SpeedText.text = speed.ToString() + " MPH";
    }
}
