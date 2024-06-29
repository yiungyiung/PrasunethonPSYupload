using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RayCasting : MonoBehaviour
{
    Camera cam;
    public LayerMask mask;
    public GameObject raystarter;
    public GameObject spot;
    public GameObject leg;
    public int currentind;
    [SerializeField]
    Button painintensity;
    [SerializeField]
    Button paintype;
    public Dictionary<int, Dictionary<string, string>> myDict = new Dictionary<int, Dictionary<string, string>>();
    Dictionary<string, string> innerDict = new Dictionary<string, string>();
    void Start()
    {
    currentind=-1;
        cam = Camera.main;
    }

    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            PerformRaycast(cam.ScreenPointToRay(Input.GetTouch(0).position));
        }
        else if (Input.GetMouseButtonDown(1))  // Right mouse button click
        {
            PerformRaycast(cam.ScreenPointToRay(Input.mousePosition));
        }
    }

    void PerformRaycast(Ray ray)
    {
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
        {   
            
            myDict.Add(currentind,innerDict);
            innerDict = new Dictionary<string, string>();
            currentind++;
            Debug.Log("Hit object: " + hit.point);
            
            // Calculate the normal direction of the hit point
            Vector3 normalDirection = hit.normal;
            
            // Spawn the object slightly above the hit point in the normal direction
            Vector3 spawnPosition = hit.point + normalDirection * 0.01f;
            
            // Instantiate the spot object and set its parent to "leg"
            var red = Instantiate(spot, spawnPosition, Quaternion.identity);
            red.transform.parent = leg.transform;
            var runscript=red.GetComponent<starteffect>();
            runscript.joint=leg;
            runscript.storedangle=leg.transform.localRotation.x;
            runscript.ps.Play();
            string localPositionString = $"{red.transform.localPosition.x:F6}, {red.transform.localPosition.y:F6}, {red.transform.localPosition.z:F6}";
            addpainlocation(localPositionString);
            painintensity.interactable=true;
            paintype.interactable=true;
            // Align the spot's up direction with the normal direction
            //red.transform.up = normalDirection;
        }

        // Draw a debug line to visualize the ray
        Debug.DrawLine(ray.origin, hit.point, Color.blue, 1.0f);
    }
    public void addpainintenstiy(string painint, string strangle, string endangle="NA")
    {
        innerDict.Add("Intensity",painint+","+strangle+","+endangle);
    }
    public void addpaindescription(string des)
    {
        innerDict.Add("Description",des);
    }
    public void addpainlocation(string location)
    {
        innerDict.Add("Location",location);
    }
    public void pushlast()
    {
        myDict.Add(currentind,innerDict);
            innerDict = new Dictionary<string, string>();
            currentind++;
    }


}
