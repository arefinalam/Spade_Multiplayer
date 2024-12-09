using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ForgetPasswordViewController : MonoBehaviour
{
    public HomeUIController homeUIController;
    public GameObject loginPanel;
    public TMP_InputField emailInputField;
    public Button submitButton;
    public TextMeshProUGUI messageText; // Using TextMeshPro for error message

    public Button backButton;

    void Start()
    {
        submitButton.onClick.AddListener(OnSubmitButtonClick);
        backButton.onClick.AddListener(onBackButtonClick);
    }

    void OnSubmitButtonClick()
    {
        messageText.text = "Password recover link has been sent to your email";
    }

    void onBackButtonClick()
    {
        homeUIController.ShowLoginPanel();
        this.gameObject.SetActive(false);
    }
}
