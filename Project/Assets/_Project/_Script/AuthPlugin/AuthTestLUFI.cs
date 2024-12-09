using FirebaseWebGL.Examples.Utils;
using FirebaseWebGL.Scripts.FirebaseBridge;
using FirebaseWebGL.Scripts.Objects;
using System;
using TMPro;
using UnityEngine;

public class AuthTestLUFI : MonoBehaviour
{
    public TMP_InputField emailText;
    public TMP_InputField passwordText;

    public TMP_Text logText;
    public TMP_Text outputText;

    private void Start()
    {
        if (Application.platform != RuntimePlatform.WebGLPlayer) return;

        //FirebaseAuth.OnAuthStateChanged(gameObject.name, nameof(OnUserSignIn), nameof(OnUserSignOut));
    }

    public void Btn_Signup()
    {
        FirebaseAuth.CreateUserWithEmailAndPassword(emailText.text, passwordText.text, gameObject.name, nameof(OnUserSignup), nameof(OnUserSignupFail));
    }

    public void Btn_Login()
    {
        FirebaseAuth.SignInWithEmailAndPassword(emailText.text, passwordText.text, gameObject.name, nameof(OnUserLogin), nameof(OnUserLoginFail));
    }

    public void Btn_PasswordReset()
    {
        FirebaseAuth.SendPassResetEmail(gameObject.name, emailText.text, nameof(OnResetPassSend), nameof(OnResetPassSendFail));
    }

    public void Btn_SignOut()
    {
        FirebaseAuth.SignOut(gameObject.name, nameof(OnUserSignOut), nameof(OnUserSignOutFail));
    }

    public void OnUserLogin(string info)
    {
        ShowLog("user logged in "+info);
        outputText.text = info;

        var parsedUser = StringSerializationAPI.Deserialize(typeof(AuthCallbackData), info) as AuthCallbackData;
        outputText.text = parsedUser.user.uid;
    }

    public void OnUserLoginFail(string error)
    {
        var parsedError = StringSerializationAPI.Deserialize(typeof(FirebaseError), error) as FirebaseError;
        ShowLog($"login failed. Error: {parsedError.message}");
        outputText.text = parsedError.message;
    }

    public void OnUserSignup(string info)
    {
        ShowLog("user signed up successful " + info);
        var parsedUser = StringSerializationAPI.Deserialize(typeof(AuthCallbackData), info) as AuthCallbackData;
        outputText.text = parsedUser.user.uid;
    }

    public void OnUserSignupFail(string error)
    {
        var parsedError = StringSerializationAPI.Deserialize(typeof(FirebaseError), error) as FirebaseError;        
        ShowLog($"login failed. Error: {parsedError.message}");
        outputText.text = error;
    }

    public void OnUserSignIn(string user)
    {
        var parsedUser = StringSerializationAPI.Deserialize(typeof(AuthCallbackData), user) as AuthCallbackData;
        ShowLog($"User signed in success. Email: {parsedUser.user.email}, UserId: {parsedUser.user.uid}, EmailVerified: {parsedUser.user.isEmailVerified}");
    }

    public void OnResetPassSend(string info)
    {
        ShowLog("pass reset success: " + info);
        outputText.text = info;
    }

    public void OnResetPassSendFail(string info)
    {
        ShowLog("pass reset failed: " + info);
        outputText.text = info;
    }

    public void OnUserSignOut(string info)
    {
        ShowLog("User signed out " + info);
        outputText.text = info;
    }

    public void OnUserSignOutFail(string info)
    {
        ShowLog("User signed out failed" + info);
        outputText.text = info;
    }

    void ShowLog(string message, bool error = false)
    {
        logText.text = message;
    }

    [Serializable]
    public class AuthCallbackData
    {
        public FirebaseUser user;
    }
}
