using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class canvascamerascript : MonoBehaviour
{
    void Start()
    {
        // Get reference to the canvas
        Canvas canvas = GetComponent<Canvas>();

        // Get reference to the main camera
        Camera mainCamera = Camera.main;

        // Set the render camera of the canvas to the main camera
        canvas.worldCamera = GameObject.FindWithTag("Deptcamera").GetComponent<Camera>();
    }
}
