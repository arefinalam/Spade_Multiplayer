using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SignupViewController : MonoBehaviour
{
    public HomeUIController homeUIController;
    public GameObject signupPanel;
    public TMP_InputField nameInputField;
    public TMP_InputField emailInputField;
    public TMP_InputField passwordInputField;
    public Button signupButton;
    public TextMeshProUGUI errorMessageText; // Using TextMeshPro for error message

    public GameObject loadingAnimation; // Reference to loading animation object
    public Button backButton;

    void Start()
    {
        loadingAnimation.SetActive(false);
        signupButton.onClick.AddListener(OnSignupButtonClick);
        backButton.onClick.AddListener(onBackButtonClick);
    }

    void OnSignupButtonClick()
    {
        //loadingAnimation.SetActive(true);
        errorMessageText.text = "";
        // Trigger signup logic with email and password input
        string email = emailInputField.text;
        string password = passwordInputField.text;
        string displayName = nameInputField.text;
        // Call SignupController method to handle signup logic
        AuthenticationController.Instance.Signup(displayName, email, password, errorMessageText, () =>
        {
            // On signup success, deactivate loading animation and navigate to main panel
            //loadingAnimation.SetActive(false);
            homeUIController.ShowMainPanel();
        });
    }
    void onBackButtonClick()
    {
        homeUIController.ShowAuthPanel();
    }
}
