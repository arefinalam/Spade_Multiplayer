using FirebaseWebGL.Examples.Utils;
using FirebaseWebGL.Scripts.Objects;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public GameData gameData;
    private bool isLoggedIn;

    private UserGameData userGameData;
    private int currentBet;

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
    }

    // Static method to access the singleton instance
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("Game is not initialized!");
            }
            return instance;
        }
    }

    void Start()
    {
        InitializeGame();
    }

    void InitializeGame()
    {

    }

    public bool IsUserLoggedIn
    {
        get { return isLoggedIn; }   
        set { isLoggedIn = value; }
    }

    public UserGameData User
    {
        get { return userGameData; }
        set { userGameData = value; }
    }

    public int CurrentBetAmount
    {
        get { return currentBet; }
        set { currentBet = value; }
    }
}
