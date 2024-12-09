using UnityEngine;
using FirebaseWebGL.Scripts.Objects;
using FirebaseWebGL.Scripts.FirebaseBridge;
using FirebaseWebGL.Examples.Utils;
using System;

public class FirebaseAuthManager : MonoBehaviour
{
    Action<UserGameData> OnUserSigninCallback;
    private string userDisplayName;

    private void Start()
    {
        if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            LogManager.Instance.ConsoleLog("The code is not running on a WebGL build; as such, the Javascript functions will not be recognized.");
            return;
        }

        FirebaseAuth.OnAuthStateChanged(gameObject.name, nameof(OnUserSignIn), nameof(OnUserSignOut));
    }

    public void OnUserSignIn(string user)
    {
        var parsedUser = StringSerializationAPI.Deserialize(typeof(FirebaseUser), user) as FirebaseUser;
        BackendController.Instance.UserDatabase.GetUserData(parsedUser.uid, AutoSigninSuccess);
    }

    private void AutoSigninSuccess(UserGameData user)
    {
        if (user == null)
        {
            LogManager.Instance.ConsoleLog("User auto logged in failed.");
            FindObjectOfType<HomeUIController>().ShowAuthPanel();
        }
        else
        {
            GameManager.Instance.IsUserLoggedIn = true;
            GameManager.Instance.User = user;
            FindObjectOfType<HomeUIController>().ShowMainPanel();
            LogManager.Instance.ConsoleLog("User auto logged in:" + user.Email);
        }
    }


    #region Signup
    public void UserSignUp(string name, string email, string password, Action<UserGameData> callback)
    {
        OnUserSigninCallback = callback;
        userDisplayName = name;
        FirebaseAuth.CreateUserWithEmailAndPassword(email, password, gameObject.name, nameof(OnUserSignupSuccess), nameof(OnUserSignupFailure));
    }

    public void OnUserSignupSuccess(string user)
    {
        var parsedUser = StringSerializationAPI.Deserialize(typeof(AuthCallbackData), user) as AuthCallbackData;
        UserGameData data = new UserGameData(userDisplayName, parsedUser.user.email, parsedUser.user.uid);
        string userDataString = JsonUtility.ToJson(data);
        BackendController.Instance.UserDatabase.CreateUser(data, SignupDatabasecallBack);
    }

    public void OnUserSignupFailure(string error)
    {
        OnUserSigninCallback?.Invoke(null);
    }

    public void SignupDatabasecallBack(UserGameData user)
    {
        if (user == null)
        {
            OnUserSigninCallback?.Invoke(null);
        }
        else
        {
            OnUserSigninCallback?.Invoke(user);
        }
    }

    #endregion

    #region Login
    public void UserLoginEmail(string email, string password, Action<UserGameData> callback)
    {
        OnUserSigninCallback = callback;
        FirebaseAuth.SignInWithEmailAndPassword(email, password, gameObject.name, nameof(OnUserLoginSuccess), nameof(OnUserLoginFailure));
    }

    private void OnUserLoginSuccess(string user)
    {
        LogManager.Instance.ConsoleLog("login raw data: " + user);
        var parsedUser = StringSerializationAPI.Deserialize(typeof(AuthCallbackData), user) as AuthCallbackData;
        LogManager.Instance.ConsoleLog("login parsed UID: " +  parsedUser.user.uid);
        BackendController.Instance.UserDatabase.GetUserData(parsedUser.user.uid, SigninDatabasecallBack);
    }
    private void OnUserLoginFailure()
    {
        OnUserSigninCallback?.Invoke(null);
    }

    public void SigninDatabasecallBack(UserGameData user)
    {
        if (user == null)
        {
            OnUserSigninCallback?.Invoke(null);
        }
        else
        {
            OnUserSigninCallback?.Invoke(user);
        }
    }

    #endregion

    #region Logout
    public void OnUserSignOut()
    {
        FirebaseAuth.SignOut(this.gameObject.name, nameof(SignOutSuccess), nameof(SignOutFailed));
    }
    private void SignOutSuccess()
    {
        LogManager.Instance.ConsoleLog("Signout Done!");
        GameManager.Instance.IsUserLoggedIn = false;
        GameManager.Instance.User = null;
        FindObjectOfType<HomeUIController>().ShowLoginPanel();
    }
    private void SignOutFailed()
    {
        LogManager.Instance.ConsoleLog("Signout Failed!");
    }
    #endregion

    [Serializable]
    public class AuthCallbackData
    {
        public FirebaseUser user;
    }
}
