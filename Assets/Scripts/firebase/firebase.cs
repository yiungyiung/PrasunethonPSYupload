using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using UnityEngine;
using UnityEngine.SceneManagement;
public class firebase : MonoBehaviour
{
    private DatabaseReference reference;
    public static firebase Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            reference = FirebaseDatabase.DefaultInstance.RootReference;
        });
    }

    public async Task CreateNewUser(string name, string age, string gender)
    {
        FirebaseUser user = FirebaseAuthManager.Instance.GetCurrentUser();
        if (user == null)
        {
            Debug.LogError("No user is signed in.");
            return;
        }
        string userId = user.UserId;
        DatabaseReference userReference = reference.Child("users").Child(userId);
        DatabaseReference patientReference = userReference.Child("Patient_Details");
        await patientReference.Child("Name").SetValueAsync(name);
        await patientReference.Child("Age").SetValueAsync(age);
        await patientReference.Child("Gender").SetValueAsync(gender);

        // Add authentication-related information
        await patientReference.Child("AuthUID").SetValueAsync(userId);
        await patientReference.Child("Email").SetValueAsync(user.Email);
        await patientReference.Child("CreationTime").SetValueAsync(user.Metadata.CreationTimestamp);
    }
    public async Task AddDataEntry(List<Dictionary<string, object>> yourList)
    {
        FirebaseUser user = FirebaseAuthManager.Instance.GetCurrentUser();
        if (user == null)
        {
            Debug.LogError("No user is signed in.");
            return;
        }
        string userId = user.UserId;
        DatabaseReference userReference = reference.Child("users").Child(userId);
        DatabaseReference jointReference = userReference.Child("Joint");
        var time = DateTime.Now;
        string timestamp = time.ToString("M-d-yyyy h:mm:ss tt");
        string activeSceneName = SceneManager.GetActiveScene().name;
        DatabaseReference exerciseReference = jointReference.Child(activeSceneName).Child(timestamp);
        DatabaseReference dataReference = exerciseReference.Child("data");
        foreach (var entry in yourList)
        {
            DatabaseReference entryNode = dataReference.Push();
            foreach (var keyValuePair in entry)
            {
                if (keyValuePair.Value is string)
                {
                    await entryNode.Child(keyValuePair.Key).SetValueAsync(keyValuePair.Value.ToString());
                }
                else
                {
                    DatabaseReference painNode = entryNode.Child("pain");
                    foreach (var dict in (Dictionary<string, object>)keyValuePair.Value)
                    {
                        await painNode.Child(dict.Key).SetValueAsync(dict.Value.ToString());
                    }
                }
            }
        }
    }


        public async Task<bool> UserProfileExists()
    {
        FirebaseUser user = FirebaseAuthManager.Instance.GetCurrentUser();
        if (user == null)
        {
            Debug.LogError("No user is signed in.");
            return false;
        }

        string userId = user.UserId;
        DatabaseReference userReference = reference.Child("users").Child(userId);

        DataSnapshot snapshot = await userReference.GetValueAsync();
        return snapshot.Exists;
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
