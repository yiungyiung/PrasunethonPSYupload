using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderValueText : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _sliderText;

    private string[] painLevels = { "No pain", "Manageable pain", "Moderate pain", "Painful", "Very painful" };

    void Start()
    {
        _slider.onValueChanged.AddListener(UpdateText);
        UpdateText(_slider.value);
    }

    void UpdateText(float val)
    {
        int index = Mathf.RoundToInt(val); // Round the value to the nearest integer
        index = Mathf.Clamp(index, 0, painLevels.Length - 1); // Ensure index is within range
        _sliderText.text = painLevels[index];
    }
}
