using System;
using System.Threading.Tasks;
using Firebase.Auth;
using UnityEngine;

public class FirebaseAuthManager : MonoBehaviour
{
    public static FirebaseAuthManager Instance { get; private set; }
    
    private FirebaseAuth auth;

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
        auth = FirebaseAuth.DefaultInstance;
    }

public async Task<FirebaseUser> SignInWithEmailPassword(string email, string password)
{
    try
    {
        TaskCompletionSource<FirebaseUser> signInCompleted = new TaskCompletionSource<FirebaseUser>();
        
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                signInCompleted.SetCanceled();
            }
            else if (task.IsFaulted)
            {
                signInCompleted.SetException(task.Exception);
            }
            else
            {
                FirebaseUser newUser = task.Result.User; // Corrected line
                signInCompleted.SetResult(newUser);
            }
        });

        return await signInCompleted.Task;
    }
    catch (Exception e)
    {
        Debug.LogError($"Failed to sign in with email/password: {e.Message}");
        return null;
    }
}


    public FirebaseUser GetCurrentUser()
    {
        return auth.CurrentUser;
    }
}