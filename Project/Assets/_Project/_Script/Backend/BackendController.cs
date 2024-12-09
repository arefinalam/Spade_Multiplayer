using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackendController : MonoBehaviour
{
    // Singleton instance
    private static BackendController instance;

    // Firebase authentication manager
    public FirebaseAuthManager AuthManager { get; private set; }

    // Firebase user database controller
    public FirebaseUserDatabaseController UserDatabase { get; private set; }
    //public FirebaseMatchDataDatabaseController MatchDataDatabase { get; private set; }

    private void Awake()
    {
        // Ensure only one instance of the BackendController exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Initialize Firebase authentication manager
        AuthManager = GetComponent<FirebaseAuthManager>();

        // Initialize Firebase user database controller
        //UserDatabase = GetComponent<FirebaseUserDatabaseController>();
        //MatchDataDatabase = GetComponent<FirebaseMatchDataDatabaseController>();
    }

    private void Start()
    {
        UserDatabase = GetComponent<FirebaseUserDatabaseController>();
    }

    // Static method to access the singleton instance
    public static BackendController Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("BackendController is not initialized!");
            }
            return instance;
        }
    }

    private void OnApplicationQuit()
    {
        //BackendController.Instance.UserDatabase.SetUserStatus(false, false, false);
    }

}
