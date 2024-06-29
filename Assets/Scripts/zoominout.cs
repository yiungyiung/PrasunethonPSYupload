using UnityEngine;

public class zoominout : MonoBehaviour
{
    public GameObject Object;

    public void ZoomIn()
    {
        // Increase the scale of the object to zoom in
        Object.transform.localScale *= 1.1f; // You can adjust the zoom factor as needed
    }

    public void ZoomOut()
    {
        // Decrease the scale of the object to zoom out
        Object.transform.localScale /= 1.1f; // You can adjust the zoom factor as needed
    }
}
