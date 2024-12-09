using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
public enum LogType { Log, Warning, Error}

public class LogManager : MonoBehaviour
{
    public static LogManager Instance;

    public bool devMode = true; // Set this to true if in dev mode
    public GameObject logPanelCanvas;
    public Button consoleButton;
    public Button warningButton;
    public Button errorButton;
    public Button copyButton;
    public Button emailButton;

    public GameObject consolePanel;
    public Transform consoleContentParent;
    public GameObject warningPanel;
    public Transform warningContentParent;
    public GameObject errorPanel;
    public Transform errorContentParent;

    public GameObject logTextPrefabs;

    private List<string> normalLogs = new List<string>();
    private List<string> warningLogs = new List<string>();
    private List<string> errorLogs = new List<string>();
    private int normalIndex = 0;
    private int warningIndex = 0;
    private int errorIndex = 0;

    private void Awake()
    {
        // Ensure only one instance of the BackendController exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        logPanelCanvas.SetActive(false);
        consoleButton.onClick.AddListener(OnConsoleButtonClick);
        warningButton.onClick.AddListener(OnwarningButtonClick);
        errorButton.onClick.AddListener(OnErrorButtonClick);
        copyButton.onClick.AddListener(OnCopyButtonClick);
        // Register to Unity's log message received event
        Application.logMessageReceived += LogUnityMessage;
    }

    void Updatee()
    {
        // Toggle log panel on double click or '~' key press
        if ((GameManager.Instance.gameData.currentMode == GameMode.Dev ||
            GameManager.Instance.gameData.currentMode == GameMode.QA) &&
            Input.touchCount >= 2)
        {
            ToggleLogPanel();
        }
        else if (Input.GetKeyDown(KeyCode.BackQuote)) // For PC browser, '~' key is KeyCode.BackQuote
        {
            ToggleLogPanel();
        }
    }

    #region System Logs
    private void OnDestroy()
    {
        // Unregister from Unity's log message received event
        Application.logMessageReceived -= LogUnityMessage;
    }

    // Method to handle Unity's log messages
    void LogUnityMessage(string logString, string stackTrace, UnityEngine.LogType type)
    {
        //Log(logString, ConvertUnityLogType(type));
    }

    // Method to convert Unity's log type to custom LogType enum
    private LogType ConvertUnityLogType(UnityEngine.LogType unityLogType)
    {
        switch (unityLogType)
        {
            case UnityEngine.LogType.Assert:
            case UnityEngine.LogType.Error:
            case UnityEngine.LogType.Exception:
                return LogType.Error;
            case UnityEngine.LogType.Warning:
                return LogType.Warning;
            default:
                return LogType.Log;
        }
    }
    #endregion

    void ToggleLogPanel()
    {
        bool isActive = logPanelCanvas.activeSelf;
        logPanelCanvas.SetActive(!logPanelCanvas.activeSelf);
    }
    private void OnConsoleButtonClick()
    {
        consolePanel.SetActive(true);
        warningPanel.SetActive(false);
        errorPanel.SetActive(false);
    }
    private void OnwarningButtonClick()
    {
        consolePanel.SetActive(false);
        warningPanel.SetActive(true);
        errorPanel.SetActive(false);
    }
    private void OnErrorButtonClick()
    {
        consolePanel.SetActive(false);
        warningPanel.SetActive(false);
        errorPanel.SetActive(true);
    }
    private void OnCopyButtonClick()
    {
        CopyLogToClipboard(LogType.Error);
        CopyLogToClipboard(LogType.Warning);
        CopyLogToClipboard(LogType.Log);
    }

    public void Log(string logMessage, LogType logType)
    {
        switch (logType)
        {
            case LogType.Log:
                ConsoleLog(logMessage);
                break;
            case LogType.Warning:
                WarningLog(logMessage);
                break;
            case LogType.Error:
                ErrorLog(logMessage);
                break;
        }
    }

    public void ConsoleLog(string message)
    {
        string logMessage = "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + message;
        normalLogs.Add(logMessage);
        GameObject textObj = Instantiate(logTextPrefabs, consoleContentParent);
        textObj.GetComponent<TMP_Text>().text = logMessage;
        Debug.Log(logMessage);
    }
    public void WarningLog(string message)
    {
        string logMessage = "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + message;
        warningLogs.Add(logMessage);
        Debug.LogWarning(logMessage);
        GameObject textObj = Instantiate(logTextPrefabs, warningContentParent);
        textObj.GetComponent<TMP_Text>().text = logMessage;
    }
    public void ErrorLog(string message)
    {
        string logMessage = "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + message;
        errorLogs.Add(logMessage);
        Debug.LogError(logMessage);
        GameObject textObj = Instantiate(logTextPrefabs, errorContentParent);
        textObj.GetComponent<TMP_Text>().text = logMessage;
    }

    public void ViewNextLog(LogType logType)
    {
        switch (logType)
        {
            case LogType.Log:
                if (normalIndex < normalLogs.Count - 1)
                    normalIndex++;
                break;
            case LogType.Warning:
                if (warningIndex < warningLogs.Count - 1)
                    warningIndex++;
                break;
            case LogType.Error:
                if (errorIndex < errorLogs.Count - 1)
                    errorIndex++;
                break;
        }

        UpdateLogText(logType);
    }

    public void ViewPreviousLog(LogType logType)
    {
        switch (logType)
        {
            case LogType.Log:
                if (normalIndex > 0)
                    normalIndex--;
                break;
            case LogType.Warning:
                if (warningIndex > 0)
                    warningIndex--;
                break;
            case LogType.Error:
                if (errorIndex > 0)
                    errorIndex--;
                break;
        }

        UpdateLogText(logType);
    }

    void UpdateLogText(LogType logType)
    {
        StringBuilder sb = new StringBuilder();
        switch (logType)
        {
            case LogType.Log:
                sb.Append("-----Normal Log-----\n");
                if (normalLogs.Count > 0)
                    sb.Append(normalLogs[normalIndex] + "\n");
                else
                    sb.Append("No Logs\n");
                sb.Append("-----Log end-----");
                break;
            case LogType.Warning:
                sb.Append("-----Warning Log-----\n");
                if (warningLogs.Count > 0)
                    sb.Append(warningLogs[warningIndex] + "\n");
                else
                    sb.Append("No Logs\n");
                sb.Append("-----Log end-----");
                break;
            case LogType.Error:
                sb.Append("-----Error Log-----\n");
                if (errorLogs.Count > 0)
                    sb.Append(errorLogs[errorIndex] + "\n");
                else
                    sb.Append("No Logs\n");
                sb.Append("-----Log end-----");
                break;
        }
    }


    public void CopyLogToClipboard(LogType logType)
    {
        StringBuilder sb = new StringBuilder();
        switch (logType)
        {
            case LogType.Log:
                sb.Append("-----Normal Log-----\n");
                foreach (string log in normalLogs)
                {
                    sb.AppendLine(log);
                }
                sb.Append("-----Log end-----");
                break;
            case LogType.Warning:
                sb.Append("-----Warning Log-----\n");
                foreach (string log in warningLogs)
                {
                    sb.AppendLine(log);
                }
                sb.Append("-----Log end-----");
                break;
            case LogType.Error:
                sb.Append("-----Error Log-----\n");
                foreach (string log in errorLogs)
                {
                    sb.AppendLine(log);
                }
                sb.Append("-----Log end-----");
                break;
        }

        GUIUtility.systemCopyBuffer = sb.ToString();
    }

    public void EmailLog(LogType logType)
    {
        string subject = "Log";
        StringBuilder body = new StringBuilder();

        switch (logType)
        {
            case LogType.Log:
                subject += " - Normal Log";
                body.AppendLine("-----Normal Log-----");
                foreach (string log in normalLogs)
                {
                    body.AppendLine(log);
                }
                body.AppendLine("-----Log end-----");
                break;
            case LogType.Warning:
                subject += " - Warning Log";
                body.AppendLine("-----Warning Log-----");
                foreach (string log in warningLogs)
                {
                    body.AppendLine(log);
                }
                body.AppendLine("-----Log end-----");
                break;
            case LogType.Error:
                subject += " - Error Log";
                body.AppendLine("-----Error Log-----");
                foreach (string log in errorLogs)
                {
                    body.AppendLine(log);
                }
                body.AppendLine("-----Log end-----");
                break;
        }

        // Replace with your email handling logic
        Debug.Log("Email Subject: " + subject + "\nEmail Body: " + body.ToString());
    }
}
