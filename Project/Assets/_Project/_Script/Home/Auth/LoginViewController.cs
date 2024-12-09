using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginViewController : MonoBehaviour
{
    public HomeUIController homeUIController;
    public GameObject loginPanel;
    public GameObject forgetPasswordPanel;
    public TMP_InputField emailInputField;
    public TMP_InputField passwordInputField;
    public Button submitButton;
    public Button forgetPasswordButton;
    public TextMeshProUGUI errorMessageText; // Using TextMeshPro for error message

    public GameObject loadingAnimation; // Reference to loading animation object
    public Button backButton;
    public Button signupButton;

    void Start()
    {
        loadingAnimation.SetActive(false);
        submitButton.onClick.AddListener(OnLoginButtonClick);
        signupButton.onClick.AddListener(OnSignupButtonClick);
        forgetPasswordButton.onClick.AddListener(OnForgetButtonClick);
        backButton.onClick.AddListener(OnBackButtonClick);
    }

    void OnLoginButtonClick()
    {
        // Show loading animation
        //loadingAnimation.SetActive(true);
        errorMessageText.text = "";
        // Trigger login logic with email and password input
        string email = emailInputField.text;
        string password = passwordInputField.text;

        // Call LoginController method to handle login logic
        AuthenticationController.Instance.Login(email, password, errorMessageText, () =>
        {
            // On login success, deactivate loading animation and navigate to main panel
            LogManager.Instance.ConsoleLog("Login Success:" + GameManager.Instance.User.Email);
            loadingAnimation.SetActive(false);
            homeUIController.ShowMainPanel();
        });
    }

    void OnBackButtonClick()
    {
        homeUIController.ShowAuthPanel();
    }

    void OnSignupButtonClick()
    {
        homeUIController.ShowSignupPanel();
    }
    void OnForgetButtonClick()
    {
        forgetPasswordPanel.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
