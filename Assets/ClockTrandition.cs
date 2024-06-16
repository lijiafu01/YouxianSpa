using System;
using UnityEngine;

public class Clock : MonoBehaviour
{
    public GameObject hourHandPivot;
    public GameObject minuteHandPivot;
    public GameObject secondHandPivot;

    private const float hoursToDegrees = -30f, minutesToDegrees = -6f, secondsToDegrees = -6f;

    private void Update()
    {
        DateTime time = DateTime.Now;
        hourHandPivot.transform.localRotation = Quaternion.Euler(0f, 0f, hoursToDegrees * time.Hour);
        minuteHandPivot.transform.localRotation = Quaternion.Euler(0f, 0f, minutesToDegrees * time.Minute);
        secondHandPivot.transform.localRotation = Quaternion.Euler(0f, 0f, secondsToDegrees * time.Second);
    }
}
