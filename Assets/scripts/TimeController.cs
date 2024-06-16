using UnityEngine;
using UnityEngine.UI;
using System;

public class TimeController : MonoBehaviour
{
    public Dropdown startHourDropdown;
    public Dropdown startMinuteDropdown;
    public Dropdown endHourDropdown;
    public Dropdown endMinuteDropdown;

    void Start()
    {
        SetUpTimeDropdowns();
        startHourDropdown.onValueChanged.AddListener(delegate { SaveStartTime(); });
        startMinuteDropdown.onValueChanged.AddListener(delegate { SaveStartTime(); });
        endHourDropdown.onValueChanged.AddListener(delegate { SaveEndTime(); });
        endMinuteDropdown.onValueChanged.AddListener(delegate { SaveEndTime(); });
    }

    public void SaveStartTime()
    {
        int hour = int.Parse(startHourDropdown.options[startHourDropdown.value].text);
        int minute = int.Parse(startMinuteDropdown.options[startMinuteDropdown.value].text);
        MainController.Instance.hour = hour;
        MainController.Instance.minute = minute;
        MainController.Instance.StartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, minute, 0);
    }

    public void SaveEndTime()
    {
        int hour = int.Parse(endHourDropdown.options[endHourDropdown.value].text);
        int minute = int.Parse(endMinuteDropdown.options[endMinuteDropdown.value].text);
        MainController.Instance.EndTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, minute, 0);
    }

    private void SetUpTimeDropdowns()
    {
        startHourDropdown.options.Clear();
        startMinuteDropdown.options.Clear();
        endHourDropdown.options.Clear();
        endMinuteDropdown.options.Clear();

        for (int i = 0; i < 24; i++)
        {
            startHourDropdown.options.Add(new Dropdown.OptionData(i.ToString("D2")));
            endHourDropdown.options.Add(new Dropdown.OptionData(i.ToString("D2")));
        }

        for (int i = 0; i < 60; i += 5)
        {
            startMinuteDropdown.options.Add(new Dropdown.OptionData(i.ToString("D2")));
            endMinuteDropdown.options.Add(new Dropdown.OptionData(i.ToString("D2")));
        }
    }
}
