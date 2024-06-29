using System;
using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using UnityEngine;
using UnityEngine.SceneManagement;

public class firebase : MonoBehaviour
{
    private DatabaseReference reference;

    private void Start()
    {
        FirebaseApp
            .CheckAndFixDependenciesAsync()
            .ContinueWith(task =>
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                reference = FirebaseDatabase.DefaultInstance.RootReference;
            });
    }

    public string AddDataEntry(
        string name,
        List<Dictionary<string, object>> yourList,
        string age,
        string gender
    )
    {
        DatabaseReference userReference;
        var time = DateTime.Now;
        string timestamp = time.ToString("M-d-yyyy h:mm:ss tt");

        System.Random random = new System.Random();
        int uniqueId = random.Next(100000, 999999);
        string userId = uniqueId.ToString();
        Debug.Log(userId);
        userReference = reference.Child(userId);
        DatabaseReference jointReference = userReference.Child("Joint");
        DatabaseReference patientReference =
            userReference.Child("Patient_Details");

        patientReference.Child("Name").SetValueAsync(name);
        patientReference.Child("Age").SetValueAsync(age);
        patientReference.Child("Gender").SetValueAsync(gender);

        string activeSceneName = SceneManager.GetActiveScene().name;
        DatabaseReference exerciseReference =
            jointReference.Child(activeSceneName).Child(timestamp);
        DatabaseReference dataReference = exerciseReference.Child("data");

        foreach (var entry in yourList)
        {
            DatabaseReference entryNode = dataReference.Push(); // Use Push to generate a unique key for each entry

            // Loop through the key-value pairs in the entry dictionary
            foreach (var keyValuePair in entry)
            {
                // Check if the value is a dictionary itself
                if (keyValuePair.Value is string)
                {
                    entryNode
                        .Child(keyValuePair.Key)
                        .SetValueAsync(keyValuePair.Value.ToString());
                }
                else
                {
                    foreach (var InnerkeyValuePair in entry)
                    {
                        DatabaseReference painNode = entryNode.Child("pain");
                        foreach (var
                            dict
                            in
                            (Dictionary<string, object>)InnerkeyValuePair.Value
                        )
                        {
                            painNode
                                .Child(dict.Key)
                                .SetValueAsync(dict.Value.ToString());
                        }
                    }
                }
            }
        }
        return userId;
        /*
        var time = DateTime.Now;
        string timestamp = time.ToString("M-d-yyyy h:mm:ss tt");
        string userId = name;
        DatabaseReference userReference = reference.Child(userId);
        DatabaseReference entryReference = userReference.Push(); // Use Push to generate a unique key

        entryReference.Child("timestamp").SetValueAsync(timestamp);
        entryReference.Child("age").SetValueAsync(age);
        entryReference.Child("gender").SetValueAsync(gender);
        entryReference.Child("scene").SetValueAsync(SceneManager.GetActiveScene().name);

        // Add yourList as child nodes under the "data" node
        DatabaseReference dataReference = entryReference.Child("data");

        foreach (var entry in yourList)
        {
            DatabaseReference entryNode = dataReference.Push(); // Use Push to generate a unique key for each entry

            foreach (var keyValuePair in entry)
            {
                entryNode.Child(keyValuePair.Key).SetValueAsync(keyValuePair.Value.ToString());
            }
        }
        */
    }

    public void AddDataEntry_old_user(
        string userId,
        List<Dictionary<string, object>> yourList
    )
    {
        DatabaseReference userReference;
        var time = DateTime.Now;
        string timestamp = time.ToString("M-d-yyyy h:mm:ss tt");
        if (string.IsNullOrEmpty(userId))
        {
            System.Random random = new System.Random();
            int uniqueId = random.Next(100000, 999999);
            userId = uniqueId.ToString();
            userReference = reference.Child(userId);
        }
        else
        {
            userReference = reference.Child(userId);
        }

        DatabaseReference jointReference = userReference.Child("Joint");

        string activeSceneName = SceneManager.GetActiveScene().name;
        DatabaseReference exerciseReference =
            jointReference.Child(activeSceneName).Child(timestamp);
        DatabaseReference dataReference = exerciseReference.Child("data");

        foreach (var entry in yourList)
        {
            DatabaseReference entryNode = dataReference.Push(); // Use Push to generate a unique key for each entry

            // Loop through the key-value pairs in the entry dictionary
            foreach (var keyValuePair in entry)
            {
                // Check if the value is a dictionary itself
                if (keyValuePair.Value is string)
                {
                    entryNode
                        .Child(keyValuePair.Key)
                        .SetValueAsync(keyValuePair.Value.ToString());
                }
                else
                {
                    foreach (var InnerkeyValuePair in entry)
                    {
                        DatabaseReference painNode = entryNode.Child("pain");
                        foreach (var
                            dict
                            in
                            (Dictionary<string, object>)InnerkeyValuePair.Value
                        )
                        {
                            painNode
                                .Child(dict.Key)
                                .SetValueAsync(dict.Value.ToString());
                        }
                    }
                }
            }
        }

        /*
       foreach (var entry in yourList)
       {
           foreach (var keyValuePair in entry)
           {
               exerciseReference.Child(keyValuePair.Key).SetValueAsync(keyValuePair.Value.ToString());
           }
       }

       var time = DateTime.Now;
       string timestamp = time.ToString("M-d-yyyy h:mm:ss tt");
       string userId = name;
       DatabaseReference userReference = reference.Child(userId);
       DatabaseReference entryReference = userReference.Push(); // Use Push to generate a unique key

       entryReference.Child("timestamp").SetValueAsync(timestamp);
       entryReference.Child("age").SetValueAsync(age);
       entryReference.Child("gender").SetValueAsync(gender);
       entryReference.Child("scene").SetValueAsync(SceneManager.GetActiveScene().name);

       // Add yourList as child nodes under the "data" node
       DatabaseReference dataReference = entryReference.Child("data");

       foreach (var entry in yourList)
       {
           DatabaseReference entryNode = dataReference.Push(); // Use Push to generate a unique key for each entry

           foreach (var keyValuePair in entry)
           {
               entryNode.Child(keyValuePair.Key).SetValueAsync(keyValuePair.Value.ToString());
           }
       }
       */
    }

    public void ReadName(
        string name,
        Action<IEnumerable<DataSnapshot>> onComplete
    )
    {
        reference
            .Child(name)
            .GetValueAsync()
            .ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    IEnumerable<DataSnapshot> childrenList = snapshot.Children;
                    onComplete?.Invoke(childrenList);
                    // Iterate through the children of the snapshot
                    /*foreach (var childSnapshot in snapshot.Children)
                {   
                    string entryId = childSnapshot.Key;
                    string entryData = childSnapshot.Value.ToString();

                    Debug.Log(entryId);
                foreach (var grandChildSnapshot in childSnapshot.Children)
                {
                    string dataEntryId = grandChildSnapshot.Key;
                    string dataEntry = grandChildSnapshot.Value.ToString();
                    Debug.Log(dataEntry);
                }
                Debug.Log("*************************************************************");
            }
            */
                }
            });
    }
}
