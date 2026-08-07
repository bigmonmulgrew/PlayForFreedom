using System;
using TMPro;
using UnityEngine;

public class SliderLabel : MonoBehaviour
{
    TextMeshProUGUI label;

    private void Awake()
    {
        label = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateLabel(Single value)
    {
        label.text = value.ToString();
    }
}
