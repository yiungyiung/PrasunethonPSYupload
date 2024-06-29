using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class progressBar : MonoBehaviour
{
    public Slider ProgressSlider;
    public float health;
    void Start()
    {

    }

    void Update()
    {
        if (ProgressSlider.value != health)
        {
            ProgressSlider.value = health;
        }
    }
}
