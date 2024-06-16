using UnityEngine;
using UnityEngine.UI;

public class ToggleControl : MonoBehaviour
{
    public Toggle oilMassageToggle;
    public Toggle dryMassageToggle;
    public Toggle oneHourToggle;
    public Toggle twoHourToggle;

    private void Start()
    {
        oilMassageToggle.isOn = false;
        dryMassageToggle.isOn = false;
        oneHourToggle.isOn = false;
        twoHourToggle.isOn = false;

        // Set up listeners for when the toggle values change
        oilMassageToggle.onValueChanged.AddListener(delegate { ToggleValueChanged(oilMassageToggle, dryMassageToggle, "ªoÀ£"); });
        dryMassageToggle.onValueChanged.AddListener(delegate { ToggleValueChanged(dryMassageToggle, oilMassageToggle, "«üÀ£"); });
        oneHourToggle.onValueChanged.AddListener(delegate { ToggleValueChanged1(oneHourToggle, twoHourToggle,1); });
        twoHourToggle.onValueChanged.AddListener(delegate { ToggleValueChanged1(twoHourToggle, oneHourToggle,2); });
    }

    // Called when a toggle value changes
    void ToggleValueChanged(Toggle changedToggle, Toggle otherToggle, string type)
    {
        if (changedToggle.isOn)
        {
            otherToggle.isOn = false;

            if (type == "ªoÀ£" || type == "«üÀ£")
            {
                MainController.Instance.Category = type;
            }
           
        }
        else
        {
            MainController.Instance.Category = "";
        }
    }
    void ToggleValueChanged1(Toggle changedToggle, Toggle otherToggle,int h)
    {
        if (changedToggle.isOn)
        {
            otherToggle.isOn = false;

          
            if (h == 1 || h == 2)
            {
                MainController.Instance.WorkTime = h;
            }
        }
        else
        {
            MainController.Instance.WorkTime = 0;
        }
    }
}
