using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance;

    public GameData gameData;

    [SerializeField] private List<PlayerController> players;
    [SerializeField] private GameplayPhoton photon;
    [SerializeField] private BidController bidController;
    [SerializeField] private RoundManager roundManager;
    [SerializeField] private TrickManager trickManager;
    [SerializeField] private CardManager cardManager;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private UIManager uIManager;

    private GameState currentState;
    private string message;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        currentState = GameState.Initialization;
        message = "";
        NextStep();
    }
    public GameplayPhoton Photon() => photon;

    public PlayerController GetSelfPlayer()
    {
        PlayerController p = players[0];
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].userId.Equals(PhotonNetwork.LocalPlayer.UserId))
            {
                p = players[i];
                break;
            }
        }
        return p;
    }

    public PlayerController GetPlayer(string uid)
    {
        PlayerController p = players[0];
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].userId.Equals(uid))
            {
                p = players[i];
                break;
            }
        }
        return p;
    }

    public PlayerController GetPlayerByTableIndex(int index)
    {
        PlayerController p = players[0];
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].tableIndex.Equals(index))
            {
                p = players[i];
                break;
            }
        }
        return p;
    }

    public Card GetCardByID(int id)
    {
        return cardManager.usableCards[id];
    }

    public CardManager CardManager()
    {
        return cardManager;
    }
    public BidController BidController()
    {
        return bidController;
    }
    public RoundManager RoundManager()
    {
        return roundManager;
    }

    public TrickManager TrickManager()
    {
        return trickManager;
    }
    public ScoreManager ScoreManager()
    {
        return scoreManager;
    }

    public UIManager UIManager()
    {
        return uIManager;
    }

    public List<PlayerController> Players()
    {
        return players;
    }

    public void UpdateStep()
    {
        currentState++;
        NextStep();
    }

    public void SetGameState(GameState state)
    {
        currentState = state;
        NextStep();
    }

    public GameState CurrentState
    {
        get
        {
            return currentState;
        }
    }

    private void NextStep()
    {
        switch (currentState)
        {
            case GameState.Initialization:
                StartCoroutine(InitializeGame());
                break;
            case GameState.Bidding:
                StartBidding();
                break;
            case GameState.TrickPlaying:
                StartTrickPlaying();
                break;
            case GameState.Scoring:
                Scoring();
                break;
            case GameState.EndGame:
                    
                break;
        }
    }

    private IEnumerator InitializeGame()
    {
        LogManager.Instance.ConsoleLog("Initializing Game...");
        DependencySetup();
        // Wait for a moment before initializing
        yield return new WaitForSeconds(1.0f);
        photon.PushPlayerData();
        // Wait for a moment before shuffling and distributing cards
        //yield return new WaitForSeconds(1.5f);
        //StartSuffleAndDistributeCards();
    }

    public void StartSuffleAndDistributeCards()
    {
        //Shuffle Card and Distribute Card
        cardManager.ShuffleDeck(); 
        //cardManager.DistributeCards();
    }


    private void DependencySetup()
    {
        bidController.Initialize();
        cardManager.Initialize();
        trickManager.Initialize();
        roundManager.Initialize();
        scoreManager.Initialize();
    }

    public void CardDistributionDone()
    {
        StartLookOutTime();
    }

    private void StartLookOutTime()
    {
        LogManager.Instance.ConsoleLog("Card Distribution Done");
        Invoke(nameof(EndLookOutTime), gameData.maxBidTime);
    }
    private void EndLookOutTime()
    {
        // Proceed to the Bidding phase
        UpdateStep();
    }

    private void StartBidding()
    {
        LogManager.Instance.ConsoleLog("Bidding Phase Started");

        // Initialize bidding phase
        bidController.StartBidPhase();
    }

    private void StartTrickPlaying()
    {
        LogManager.Instance.ConsoleLog("Trick Playing Phase Started");
        //yield return new WaitForSeconds(gameData.phaseWaitTime);
        roundManager.StartTrickPlaying();
    }

    private IEnumerator Scoring()
    {
        Dictionary<PlayerController, int> scores = scoreManager.GetFinalScores();
        yield return new WaitForSeconds(gameData.phaseWaitTime);
    }

}
