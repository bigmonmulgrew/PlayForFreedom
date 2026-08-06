using System;
using UnityEngine;
using UnityEngine.UI;

public class UIColourSelector : MonoBehaviour
{
    [SerializeField] Button defaultButton;

    [SerializeField] Button colourButton1;
    [SerializeField] Button colourButton2;
    [SerializeField] Button colourButton3;

    Button selectedButton;
    FlexibleColorPicker colorPicker;
    
    public Color PlayerColour1 => colourButton1 == null ? Color.magenta : colourButton1.colors.normalColor;
    public Color PlayerColour2 => colourButton2 == null ? Color.magenta : colourButton2.colors.normalColor;
    public Color PlayerColour3 => colourButton3 == null ? Color.magenta : colourButton3.colors.normalColor;

    private void Awake()
    {
        FindReferrences();
        
    }

    void FindReferrences()
    {
        selectedButton = defaultButton;
        selectedButton.GetComponent<Outline>().enabled = true;
        colorPicker = GetComponentInChildren<FlexibleColorPicker>();
        colorPicker.color = selectedButton.colors.normalColor;
    }

 


    public void SetSelectedButton(Button button)
    {
        if (button == null) return;

        selectedButton.GetComponent<Outline>().enabled = false;
        selectedButton = button;
        selectedButton.GetComponent<Outline>().enabled = true;

        colorPicker.color = selectedButton.colors.normalColor;

    }



    public void ColourPickerChanged(Color newColor)
    {
        if (selectedButton == null) return;

        selectedButton.colors = SetAllColours(newColor);

    }

    public ColorBlock SetAllColours(Color newColor)
    {
        ColorBlock allColours = selectedButton.colors;

        allColours.normalColor = newColor;
        allColours.pressedColor = newColor;
        allColours.selectedColor = newColor;

        // We don't disable, so this shouldbe be relevant, setting it for safety.
        allColours.disabledColor = newColor;

        allColours.highlightedColor = newColor * 0.7f;

        


        return allColours;
    }
}
