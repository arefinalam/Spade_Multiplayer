using UnityEngine;
using TMPro;
using System;

public class AuthenticationController : MonoBehaviour
{
    public static AuthenticationController Instance;

    Action OnSignup;
    Action OnLogin;
    TextMeshProUGUI errorText;

    void Awake()
    {
        Instance = this;
    }

    #region Login
    public void Login(string email, string password, TextMeshProUGUI errorMessageText, Action onLoginSuccess)
    {
        // Implement login logic here
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            errorMessageText.text = "Please enter email and password.";
            return;
        }
      
        // Example: You can use Firebase Authentication or another authentication service
        LogManager.Instance.ConsoleLog("Logging in with email: " + email + " and password: " + password);

        errorText = errorMessageText;
        OnLogin = onLoginSuccess;
        BackendController.Instance.AuthManager.UserLoginEmail(email, password, OnUserLogin);            
    }

    public void OnUserLogin(UserGameData data)
    {
        if (data == null)
        {
            //error
            errorText.text = "Invalid Email or Password! Try Again.";
            Invoke(nameof(ClearErrorText), 2f);
        }
        else
        {
            //success
            GameManager.Instance.User = data;
            OnLogin?.Invoke();
        }
    }

    #endregion

    #region Signup

    public void Signup(string name, string email, string password, TextMeshProUGUI errorMessageText, Action onSignupSuccess)
    {
        // Implement signup logic here
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            errorMessageText.text = "Please enter email and password.";
            return;
        }

        errorText = errorMessageText;
        OnSignup = onSignupSuccess;
        BackendController.Instance.AuthManager.UserSignUp(name, email, password, OnUserSignup);       
    }

    public void OnUserSignup(UserGameData data)
    {
        if (data == null)
        {
            //error
            errorText.text = "Something went wrong. Try again";
            Invoke(nameof(ClearErrorText), 2f);
        }
        else
        {
            OnSignup?.Invoke();
            GameManager.Instance.User = data;
        }
    }

    private void ClearErrorText()
    {
        errorText.text = "";
    }
    #endregion
}
