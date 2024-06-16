using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnClickEffect : MonoBehaviour
{
    public Button[] buttons;

    private struct ButtonState
    {
        public Vector3 originalScale;
        public ColorBlock originalColors;
    }

    private Dictionary<Button, ButtonState> buttonStates = new Dictionary<Button, ButtonState>();

    private void Start()
    {
        foreach (Button button in buttons)
        {
            // Save the initial state of the buttons
            ButtonState state;
            state.originalScale = button.transform.localScale;
            state.originalColors = button.colors;
            buttonStates[button] = state;

            button.onClick.AddListener(() => ChangeButtonAppearance(button));
        }
    }

    private void ChangeButtonAppearance(Button button)
    {
        // Reset all buttons to their original state
        foreach (Button otherButton in buttons)
        {
            ButtonState state = buttonStates[otherButton];
            otherButton.transform.localScale = state.originalScale;
            otherButton.colors = state.originalColors;
        }

        // Reset the size of the clicked button to original before enlarging
        ButtonState clickedButtonState = buttonStates[button];
        button.transform.localScale = clickedButtonState.originalScale;

        // Increase size of the clicked button by 1.2 times
        button.transform.localScale *= 1.2f;

        // Change color of the clicked button to gray
        ColorBlock buttonColors = button.colors;
        buttonColors.normalColor = Color.gray;
        buttonColors.highlightedColor = Color.gray;
        button.colors = buttonColors;
    }

}
