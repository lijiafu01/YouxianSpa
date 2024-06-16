using UnityEngine;
using UnityEngine.UI;
using System;

public class RealTimeClock : MonoBehaviour
{
    public Text hourText;
    public Text minuteText;
    public Text secondText;

    // Update is called once per frame
    void Update()
    {
        DateTime time = DateTime.Now;
        hourText.text = time.Hour.ToString("D2")+":";
        minuteText.text = time.Minute.ToString("D2")+":";
        secondText.text = time.Second.ToString("D2");
       
    }
}
