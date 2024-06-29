using System;
using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class readdata : MonoBehaviour
{
    public holdrotate hr;

    private int currentIndex = 0;
    public Dropdown tmpDropdown;
    [SerializeField]
    public Dropdown joint_Dropdown;
    [SerializeField]

    public Dropdown userId_Dropdown;
    [SerializeField]
    TMP_InputField Inputname;
    [SerializeField]
    TMP_Text name;
    [SerializeField]
    TMP_Text age;
    [SerializeField]
    TMP_Text gender;
    [SerializeField]
    private TMP_Text scenes;
    [SerializeField]
    private TMP_Text type;
    [SerializeField]
    private TMP_Text joint;
    [SerializeField]
    private TMP_Text angle;
    public firebase fb;

    public IEnumerable<DataSnapshot> alldata;
    public List<string> jointsAvailable = new List<string>();
    public List<string> DatedataforDropdown = new List<string>();

    public string selectedJoint;
    public Dictionary<string, List<string>> jointTimestamps = new Dictionary<string, List<string>>();
    public Dictionary<string, List<int>> jointmaxmin = new Dictionary<string, List<int>>();
    public Dictionary<string, List<DataSnapshot>> jointDataTimeSnapshot = new Dictionary<string, List<DataSnapshot>>();
    List<DataSnapshot> childlastdata = new List<DataSnapshot>();

    public List<Dictionary<string, object>>
        shotdata = new List<Dictionary<string, object>>();


    public GameObject spot;

    GameObject legpain;

    GameObject Painpoint;

    [SerializeField]
    string names = "";

    [SerializeField]
    GameObject Knee1;

    [SerializeField]
    GameObject Elbow;

    string scene;
    bool ranged = false;
    bool acute = false;
    Quaternion originalRotation;

    void Start()
    {
        StartCoroutine(GetRootNodes());
    }

    IEnumerator GetRootNodes()
    {
        var task = FirebaseDatabase.DefaultInstance.GetReference("/").GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            Debug.LogWarning($"Failed to fetch Firebase data: {task.Exception}");
            yield break;
        }
        DataSnapshot snapshot = task.Result;
        List<string> rootNodes = new List<string>();
        foreach (var childSnapshot in snapshot.Children)
        {
            rootNodes.Add(childSnapshot.Key);
        }
        foreach (var rootNode in rootNodes)
        {
            userId_Dropdown.options.Add(new Dropdown.OptionData(rootNode.ToString()));
        }
    }

    public void onuseridDropdownValueChanged()
    {
        string namess = userId_Dropdown.options[userId_Dropdown.value].text;
        fb.ReadName(namess, OnReadNameComplete);
        userId_Dropdown.interactable = false;
    }

    public void OnBackButtonClick()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void OnjointDropdownValueChanged()
    {
        selectedJoint = joint_Dropdown.options[joint_Dropdown.value].text;
        childlastdata = jointDataTimeSnapshot[selectedJoint];

        tmpDropdown.ClearOptions();
        string first = "Select an option";
        tmpDropdown.options.Add(new Dropdown.OptionData(first));
        foreach (var data in childlastdata)
        {
            tmpDropdown.options.Add(new Dropdown.OptionData(data.Key));
        }
    }
    private void OnReadNameComplete(IEnumerable<DataSnapshot> childrenList)
    {
        joint_Dropdown.ClearOptions();
        string first_joint = "Select an joint";
        joint_Dropdown.options.Add(new Dropdown.OptionData(first_joint));
        tmpDropdown.ClearOptions();
        string first = "Select an option";
        tmpDropdown.options.Add(new Dropdown.OptionData(first));

        jointmaxmin.Clear();
        jointTimestamps.Clear();
        jointsAvailable.Clear();
        jointDataTimeSnapshot.Clear();

        alldata = childrenList;

        if (alldata != null)
        {
            names = "";
            foreach (var ch in alldata)
            {
                if (ch.Key == "Patient_Details")
                {
                    DataSnapshot type = ch;
                    foreach (var deets in type.Children)
                    {
                        names = names + deets.Value + ",";
                        Debug.Log(deets.Value);
                    }
                }
            }
            foreach (var childSnapshot in alldata)//childSnapshit (joint,Patient_Details)
            {

                if (childSnapshot.Key == "Joint")
                {

                    DataSnapshot jointype = childSnapshot;
                    foreach (var jointChild in jointype.Children)// joint child: elbow,knee1,...
                    {
                        jointDataTimeSnapshot.Add(jointChild.Key.ToString(), new List<DataSnapshot>());
                        jointsAvailable.Add(jointChild.Key.ToString());
                        joint_Dropdown.options.Add(new Dropdown.OptionData(jointChild.Key.ToString()));
                        jointTimestamps.Add(jointChild.Key.ToString(), new List<string>());
                        jointmaxmin.Add(jointChild.Key.ToString(), new List<int>());
                        //if timestamp dictionary for graph does not havde that join make new key with empty list
                        if (!jointTimestamps.ContainsKey(jointChild.Key.ToString()))
                        {
                            jointTimestamps.Add(jointChild.Key.ToString(), new List<string>());
                        }
                        if (!jointmaxmin.ContainsKey(jointChild.Key.ToString()))
                        {
                            jointmaxmin.Add(jointChild.Key.ToString(), new List<int>());
                        }
                        foreach (var timestampSnapshot in jointChild.Children)//timestamp of joint
                        {
                            string timestamp1 = timestampSnapshot.Key;
                            jointTimestamps[jointChild.Key].Add(timestamp1);
                            jointDataTimeSnapshot[jointChild.Key].Add(timestampSnapshot);
                            DataSnapshot datachild = timestampSnapshot.Child("data");
                            foreach (var child in datachild.Children)
                            {
                                foreach (var valuesMin in child.Children)//for graph max min values
                                {
                                    if (valuesMin.Key == "maxmin")
                                    {
                                        string[] components = valuesMin.Value.ToString().Trim('(', ')').Split(',');
                                        jointmaxmin[jointChild.Key].Add(int.Parse(components[0]));
                                    }
                                }
                            }
                        }

                    }
                }
                joint_Dropdown.RefreshShownValue();
                tmpDropdown.RefreshShownValue();
            }
        }
    }

    public void OnDropdownValueChanged()
    {
        int index = tmpDropdown.value - 1;
        Debug.Log("childlastdata: " + childlastdata.Count);
        if (index >= 0 && index < childlastdata.Count)
        {
            shotdata.Clear();
            Debug.Log("Selected index: " + index);
            currentIndex = 0;

            // Access the corresponding DataSnapshot from the child list
            DataSnapshot selectedSnapshot = childlastdata[index];
            Debug.Log("selectedsnapshot: " + selectedSnapshot);

            // Access the "data" child
            DataSnapshot dataSnapshot = selectedSnapshot.Child("data");

            DatabaseReference parent_timestamp = selectedSnapshot.Reference.Parent;

            scene = parent_timestamp.Key.ToString();

            joint.text = "Current Joint:";
            updatedeets(names);
            foreach (var typeSnapshot in dataSnapshot.Children)
            {
                string dataType = typeSnapshot.Key;
                foreach (var entrySnapshot in typeSnapshot.Children)
                {
                    string entryKey = entrySnapshot.Key;
                    var entryValue = entrySnapshot.Value;
                    Dictionary<string, object> keyValuePairs =
                        new Dictionary<string, object> {
                            { entryKey, entryValue }
                        };
                    shotdata.Add(keyValuePairs);
                }
            }
            Knee1.SetActive(false);
            Elbow.SetActive(false);

            switch (selectedJoint)
            {
                case "knee 1":
                    Knee1.SetActive(true);
                    legpain = FindChildWithTag(Knee1, "painloc");
                    hr.hd = Knee1.GetComponent<Rotation>();
                    break;
                case "elbow":
                    Elbow.SetActive(true);
                    legpain = FindChildWithTag(Elbow, "painloc");
                    hr.hd = Elbow.GetComponent<Rotation>();
                    break;
            }
            DisplayCurrentIndex();
        }
    }

    public void updatedeets(string deet)
    {
        var deetsplit = deet.Trim().Split(',');
        Debug.Log("Updated name");
        gender.SetText(deetsplit[1]);
        age.SetText(deetsplit[0]);
        name.SetText(deetsplit[2]);
    }
    public void ReduceIndex()
    {
        if (shotdata.Count > 0)
        {
            currentIndex = (currentIndex - 1 + shotdata.Count) % shotdata.Count;
            DisplayCurrentIndex();
        }
    }

    public void IncreaseIndex()
    {
        if (shotdata.Count > 0)
        {
            currentIndex = (currentIndex + 1) % shotdata.Count;
            DisplayCurrentIndex();
        }
    }

    private void DisplayCurrentIndex()
    {
        Debug.Log($"Current Index: {currentIndex}");

        if (shotdata.Count > 0)
        {
            legpain.transform.localRotation = originalRotation;
            ranged = false;
            acute = false;
            angle.text = "";
            Dictionary<string, object> currentEntry = shotdata[currentIndex];

            if (Painpoint != null)
            {
                Destroy(Painpoint);
            }
            originalRotation = legpain.transform.localRotation;
            if (currentEntry.ContainsKey("maxmin"))
            {
                type.text = "Max ROM";
                Debug.Log("Type: MaxMin");
                // Perform actions specific to MaxMin type
                List<int> range = new List<int>();
                Debug.Log(currentEntry["maxmin"]);
                range = ParseList(currentEntry["maxmin"].ToString(), 0);
                if (range != null)
                {
                    angle.text = range[0] + "to" + range[1];
                    int init = range[0];
                    int fin = range[1];
                    ranged = true;
                    StartCoroutine(MoveLegPain(init, fin));
                }
            }
            else if (currentEntry.ContainsKey("pain"))
            {
                type.text = "Pain Position";
                Debug.Log("Type: Position");
                Debug.Log(((Dictionary<string, object>)currentEntry["pain"])["Location"]);
                Vector3 position = ParseVector3((string)((Dictionary<string, object>)currentEntry["pain"])["Location"]);
                var spotf = Instantiate(spot, position, Quaternion.identity);
                spotf.transform.parent = legpain.transform;
                spotf.transform.localPosition = position;
                spotf.transform.localScale = 4 * Vector3.one;
                Painpoint = spotf;
                string Intensity = (string)((Dictionary<string, object>)currentEntry["pain"])["Intensity"];
                if (Intensity != "NA")
                {
                    List<int> range = new List<int>();
                    range = ParseList(Intensity, 1);
                    Debug.Log("Intensity: " + Intensity);
                    if (range != null)
                    {
                        if (range.Count == 2)
                        {
                            acute = true;
                            string category = "PainIntensity: " + range[0];
                            angle.text = range[1] + " " + category;
                            StartCoroutine(AcuteLegPain(range[0]));
                        }
                        else if (range.Count == 3)
                        {
                            string category = "PainIntensity: " + range[0];
                            angle.text = range[1] + "to" + range[2] + category;
                            int init = range[1];
                            int fin = range[2];
                            ranged = true;
                            StartCoroutine(MoveLegPain(init, fin));
                        }
                    }
                }
                string Description = (string)((Dictionary<string, object>)currentEntry["pain"])["Description"];
                if (Description != "NA")
                {
                    Debug.Log(Description);
                }
            }
        }
        else
        {
            Debug.Log("No data available.");
        }
    }


    private IEnumerator AcuteLegPain(int targetAngle)
    {
        // Save the original rotation


        // Set the rotation instantly to the target angle
        legpain.transform.localEulerAngles = new Vector3(targetAngle, 0, 0);

        // Continuously check the condition (e.g., a variable change)
        while (acute) // Replace VariableChanged with your actual condition
        {
            yield return null; // Wait for the next frame
        }
        // Code here will execute after the variable changes
        Debug.Log("Leg pain rotation complete!");

        // If you need to perform any actions after the rotation, you can do it here...
    }
    private IEnumerator MoveLegPain(int init, int fin)

    {

        float duration = 2f; // Adjust the duration as needed
        float elapsed = 0f;
        Debug.Log("moving");
        while (ranged)
        {
            float t = Mathf.Sin(elapsed / duration * Mathf.PI * 2) * 0.5f + 0.5f;

            // Interpolate between init and fin
            int currentPos = (int)Mathf.Lerp(init, fin, t);

            // Move legpain to the current position
            // Replace "legpain.transform.position.x" with the actual property you want to modify
            legpain.transform.localEulerAngles = new Vector3(currentPos, 0, 0);

            elapsed += Time.deltaTime;
            yield return null;

        }

    }
    private Vector3 ParseVector3(string vectorString)
    {
        string[] components = vectorString.Trim().Split(',');

        if (components.Length == 3)
        {
            float x = float.Parse(components[0]);
            float y = float.Parse(components[1]);
            float z = float.Parse(components[2]);

            return new Vector3(x, y, z);
        }

        return Vector3.zero; //Return a default value if parsing fails
    }

    private List<int> ParseList(string str, int type)
    {
        string[] components = str.Trim('(', ')').Split(',');
        List<int> value = new List<int>();

        if (type == 0)
        {
            value.Add(int.Parse(components[0]));
            value.Add(int.Parse(components[1]));
            return value;
        }
        else if (type == 1)
        {
            if (components[2] != "NA")
            {
                value.Add(int.Parse(components[0]));
                value.Add(int.Parse(components[1]));
                value.Add(int.Parse(components[2]));
                return value;
            }
            else
            {
                value.Add(int.Parse(components[0]));
                value.Add(int.Parse(components[1]));
                return value;
            }
        }
        return null;
    }


    GameObject FindChildWithTag(GameObject parent, string tag)
    {
        Transform[] children = parent.GetComponentsInChildren<Transform>(true);

        foreach (Transform child in children)
        {
            if (
                child != null &&
                child.gameObject != null &&
                child.gameObject.tag == tag
            )
            {
                return child.gameObject;
            }
        }

        // Child with the specified tag not found
        return null;
    }
}