using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class painarray : MonoBehaviour
{
    int i = 0;
    bool up = false;
    public List<string> data = new List<string>();
    public List<Dictionary<string, object>> data1 = new List<Dictionary<string, object>>();
    [SerializeField]
    TMP_Text Text;
    [SerializeField]

    TMP_Text Textuserid;
    [SerializeField]

    TMP_InputField register_userid;
    [SerializeField]
    TMP_InputField register_username;
    [SerializeField]
    TMP_InputField age;
    [SerializeField]
    TMP_Dropdown gender;
    public firebase fb;
    public movesoham mv;

    [SerializeField]
    RayCasting ray;

    void Start()
    {
        data1.Clear();
        fb = firebase.Instance;
    }
    public void singledata(float init, string painLevel)
    {
        i = 0;
        string s = "acute pain at " + init + "°";
        Dictionary<string, object> entry = new Dictionary<string, object>
        {
            { "acute_pain", $"({init},{painLevel})" }
        };
        //data1.Add(entry);
        data.Add(s);
        up = true;
    }

    public void rangedata(float init, float last, string painLevel)
    {
        i = 0;
        Dictionary<string, object> entry = new Dictionary<string, object>
        {
            { "ranged_pain", $"({init}, {last}, {painLevel})" }
        };
        //data1.Add(entry);
        string s = "ranged pain at " + init + "° to " + last + "°";
        data.Add(s);
        up = true;
    }

    void Update()
    {
        if (up)
        {
            up = false;
            string s = "";
            if (data.Count > 5)
            {
                for (int i = data.Count - 1; i >= data.Count - 5; i--)
                {
                    s = s + data[i] + "\n";
                }
            }
            else
            {
                for (int i = data.Count - 1; i >= 0; i--)
                {
                    s = s + data[i] + "\n";
                }
            }
            Text.text = s;
        }
    }

    public void senddataoldUser()
    {
        ray.pushlast();
        foreach (var kvp in ray.myDict)
        {
            Debug.Log("Key" + kvp.Key);
            if (kvp.Key == -1)
            {
                continue;
            }
            Dictionary<string, string> innerDict = kvp.Value;

            Debug.Log("Location: " + (innerDict.ContainsKey("Location") ? innerDict["Location"] : "NA"));
            Debug.Log("Intensity: " + (innerDict.ContainsKey("Intensity") ? innerDict["Intensity"] : "NA"));
            Debug.Log("Description: " + (innerDict.ContainsKey("Description") ? innerDict["Description"] : "NA"));
            Dictionary<string, object> innerentry = new Dictionary<string, object>
            {
                //innerDict.ContainsKey("Desc") ? innerDict["Desc"] : "NA" 
                { "Location", innerDict.ContainsKey("Location") ? innerDict["Location"] : "NA" },
                { "Intensity", innerDict.ContainsKey("Intensity") ? innerDict["Intensity"] : "NA" },
                { "Description", innerDict.ContainsKey("Description") ? innerDict["Description"] : "NA" }
        };
            Dictionary<string, object> entry = new Dictionary<string, object>
            {
                { "pain", innerentry }
            };
            data1.Add(entry);
            innerDict.Clear();
        }
        GameObject[] gameObjects;
        gameObjects = GameObject.FindGameObjectsWithTag("Finish");
        foreach (GameObject gameObject in gameObjects)
        {
            Destroy(gameObject);
        }
        Dictionary<string, object> minMaxEntry = new Dictionary<string, object>
        {
            { "maxmin", $"({mv.max}, {mv.min})" },
        };
        data1.Add(minMaxEntry);
        fb.AddDataEntry(data1);
        data.Clear();
        data1.Clear();
        Text.text = "";
    }
}



