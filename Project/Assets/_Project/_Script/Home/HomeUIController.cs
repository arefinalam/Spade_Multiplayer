using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HomeUIController : MonoBehaviour
{
    public static HomeUIController Instance;

    [Header("Text")]
    public TMP_Text usernameText;
    public TMP_Text coinText;
    public TMP_Text onlinePlayerCountText;

    [Header("Button")]
    public Button loginButton;
    public Button signupButton;
    public Button homeButton;
    public Button startButton;
    public Button accountButton;
    public Button settingsButton;

    [Header("Panel")]
    public GameObject authPanel;
    public GameObject authButtonPanel;
    public GameObject loginPanel;
    public GameObject signupPanel;
    public GameObject ForgetPasswordPanel;
    public GameObject topBarPanel;
    public GameObject bottomBarPanel;
    public GameObject mainPanel;
    public GameObject betLevelPanel;
    public GameObject storePanel;
    public GameObject settingsPanel;
    public GameObject roomSelectionPanel;
    public GameObject privateRoomPanel;
    public GameObject matchmakingPanel;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Add listeners to buttons
        loginButton.onClick.AddListener(ShowLoginPanel);
        signupButton.onClick.AddListener(ShowSignupPanel);
        homeButton.onClick.AddListener(ShowMainPanel);
        startButton.onClick.AddListener(ShowBetLevelPanel);
        accountButton.onClick.AddListener(ShowStorePanel);
        settingsButton.onClick.AddListener(ShowSettingsPanel);

        loginButton.onClick.AddListener(ShowLoginPanel);
        signupButton.onClick.AddListener(ShowSignupPanel);
        startButton.onClick.AddListener(ShowBetLevelPanel);
        accountButton.onClick.AddListener(ShowStorePanel);
        settingsButton.onClick.AddListener(ShowSettingsPanel);
        ShowAuthPanel();
    }

    public void ShowAuthPanel()
    {
        ActivatePanel(authPanel);
        authButtonPanel.SetActive(true);
        ToggleTopBottomBarPanel(false);
    }

    public void ShowLoginPanel()
    {
        ActivatePanel(loginPanel);
        authPanel.SetActive(true);
        authButtonPanel.SetActive(false);
        //ShowMainPanel();
    }

    public void ShowSignupPanel()
    {
        ActivatePanel(signupPanel);
        authPanel.SetActive(true);
        authButtonPanel.SetActive(false);
        //ShowMainPanel();
    }

    public void ToggleTopBottomBarPanel(bool toggleView)
    {
        topBarPanel.SetActive(toggleView);
        bottomBarPanel.SetActive(toggleView);
    }

    public void ShowMainPanel()
    {
        usernameText.text = GameManager.Instance.User.DisplayName;
        coinText.text = GameManager.Instance.User.TotalCurrency.ToString();
        GlobalData.uid = GameManager.Instance.User.UserId;
        GlobalData.nickname = GameManager.Instance.User.DisplayName;
        ControllerPhoton.self.Init(GameManager.Instance.User.UserId, GameManager.Instance.User.DisplayName);
        ActivatePanel(mainPanel);
        ToggleTopBottomBarPanel(true);
    }

    public void ShowBetLevelPanel()
    {
        ActivatePanel(betLevelPanel);
        ToggleTopBottomBarPanel(true);
    }

    public void ShowStorePanel()
    {
        ActivatePanel(storePanel);
        ToggleTopBottomBarPanel(true);
    }

    public void ShowSettingsPanel()
    {
        ActivatePanel(settingsPanel);
        ToggleTopBottomBarPanel(true);
    }

    public void ShowMatchmakingPanel()
    {
        ActivatePanel(matchmakingPanel);
        ToggleTopBottomBarPanel(false);
    }

    public void ShowRoomSelectionPanel()
    {
        ActivatePanel(roomSelectionPanel);
        ToggleTopBottomBarPanel(false);
    }
    public void ShowPrivateRoomPanel()
    {
        ActivatePanel(privateRoomPanel);
        ToggleTopBottomBarPanel(false);
    }

    void ActivatePanel(GameObject panel)
    {
        // Deactivate all panels
        authPanel.SetActive(false);
        authButtonPanel.SetActive(false);
        loginPanel.SetActive(false);
        signupPanel.SetActive(false);
        ForgetPasswordPanel.SetActive(false);
        topBarPanel.SetActive(false);
        bottomBarPanel.SetActive(false);
        mainPanel.SetActive(false);
        betLevelPanel.SetActive(false);
        storePanel.SetActive(false);
        settingsPanel.SetActive(false);
        roomSelectionPanel.SetActive(false);
        privateRoomPanel.SetActive(false);
        matchmakingPanel.SetActive(false);
        // Activate the specified panel
        panel.SetActive(true);
    }
}
