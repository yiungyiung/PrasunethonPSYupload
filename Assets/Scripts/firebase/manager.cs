using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using Firebase.Auth;
using Google;
using TMPro;
using UnityEngine.SceneManagement;
public class manager : MonoBehaviour
{


[SerializeField]
TMP_InputField name;
[SerializeField]
TMP_InputField age;
[SerializeField]
TMP_InputField email;
[SerializeField]
TMP_InputField password;
[SerializeField]
    TMP_Dropdown gender;
 public async Task SignInAndInitialize()
    {
        FirebaseUser user = await FirebaseAuthManager.Instance.SignInWithEmailPassword(email.text, password.text);
        
        if (user != null)
        {
            bool profileExists = await firebase.Instance.UserProfileExists();

            if (!profileExists)
            {
                // You might want to get this information from the user through a UI
                await firebase.Instance.CreateNewUser(name.text, age.text ,gender.options[gender.value].text);
                Debug.Log("New user profile created.");
                SceneManager.LoadScene("homepage");
            }
            else
            {
                Debug.Log("Existing user logged in.");
                SceneManager.LoadScene("homepage");
            }
        }
        else
        {
            Debug.LogError("Failed to sign in user.");
        }
    }

    // Call this method to start the sign-in process
    public void StartSignIn()
    {
        SignInAndInitialize().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError($"SignIn failed: {task.Exception}");
            }
            else
            {
                Debug.Log("SignIn completed successfully");
            }
        });
    }
}