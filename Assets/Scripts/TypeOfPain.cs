using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TypeOfPain : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private int numberToSend;
    [SerializeField]
    RayCasting ray;
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked");
        Debug.Log("type: " + numberToSend);
        SendNumber(numberToSend);
        ray.addpaindescription(numberToSend+"");
    }

    // Method to send the number to the desired location
    private void SendNumber(int number)
    {
        switch (number)
        {
            case 1:
                Debug.Log("other");
                // Send number 1 to wherever it needs to go
                break;
            case 2:
                Debug.Log("aching");
                // Send number 2 to wherever it needs to go
                break;
            case 3:
                Debug.Log("numb");
                // Send number 3 to wherever it needs to go
                break;
            case 4:
                Debug.Log("pins and needle");
                // Send number 4 to wherever it needs to go
                break;
            case 5:
                Debug.Log("stabbing");
                // Send number 5 to wherever it needs to go
                break;
            case 6:
                Debug.Log("burn");
                // Send number 6 to wherever it needs to go
                break;
            default:
                Debug.Log("Unknown type");
                // Handle unknown types if necessary
                break;
        }
    }
}
