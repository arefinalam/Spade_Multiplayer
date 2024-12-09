using DG.Tweening;
using Helper.Extension;
using Helper.Waiter;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
public class SandboxGameplay : MonoBehaviourPun
{
    public static SandboxGameplay self;
    public SandboxCalculation calculation;
    public GameData so;
    public CardSO cardSO;

    [Header("Table")]
    public TMP_Text msgT;
    public Transform cardHolder;
    public Transform turnCardHolder;
    public SandboxPlayerData[] players;

    [Header("Bid Panel")]
    public GameObject bidPanel;
    public Slider bidTimerFill;

    [Header("Score")]
    public TMP_Text roundT;
    public ScoreUI[] scoreUi;

    [Header("GameOver Popup")]
    public GameObject popupParent;
    public GameObject popup1, popup2;
    public ScoreUI[] popupScoreUi;
    public TMP_Text winnerNameT, winnerScoreT, winnerTotalScoreT;
    public Slider winnerPanelTimer;

    //private stuff
    List<PlayerData> data;
    int myIndex = -1;
    int currentTurnIndex = 0;
    List<Card> usableCards;
    Dictionary<SandboxPlayerData, Card> cardsOnDeck;
    Dictionary<SandboxPlayerData, int> playerBids;
    Dictionary<SandboxPlayerData, int> trickWinners;
    int totalTrickCount = 0;
    Tween bidTimerTween;
    int numberOfRound = 0;
    bool isGameOver = false;

    private void Awake()
    {
        self = this;
        Init();
    }

    private void Start()
    {
        isGameOver = false;
        LoadingPanel.self.Show("Getting player data");

        if (PhotonNetwork.IsMasterClient) Invoke(nameof(PushPlayerData), 0.5f);      
    }

    void PushPlayerData()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < ControllerPhoton.self.playersInRoom.Count; i++)
            {
                Player p = ControllerPhoton.self.playersInRoom[i];
                photonView.RPC(nameof(ReceivePlayerPositionData), RpcTarget.All, i, p.NickName, p.UserId);
            }


            Invoke(nameof(PushCardData), 0.5f);   
        }
    }

    void PushCardData()
    {
        List<int> suff = new List<int>();
        for (int i = 0; i < 52; i++)
        {
            suff.Add(i);
        }
        for (int i = suff.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            int temp = suff[i];
            suff[i] = suff[randomIndex];
            suff[randomIndex] = temp;
        }

        int cardsPerPlayer = suff.Count / players.Length;
        for (int i = 0; i < players.Length; i++)
        {
            List<int> playerCards = new List<int>();
            for (int j = 0; j < cardsPerPlayer; j++)
            {
                int index = i * cardsPerPlayer + j;
                playerCards.Add(suff[index]);
            }
            playerCards.Sort();
            photonView.RPC(nameof(ReceivePlayerCardData), RpcTarget.All, players[i].uid, playerCards.ToArray());
        }
    }

    void Init()
    {
        numberOfRound = 1;
        totalTrickCount = 0;
        trickWinners = new Dictionary<SandboxPlayerData, int>();
        cardsOnDeck = new Dictionary<SandboxPlayerData, Card>();
        playerBids = new Dictionary<SandboxPlayerData, int>();
        usableCards = new List<Card>();
        for (int i = 0; i < cardSO.cards.Length; i++)
        {
            Card card = Instantiate(cardSO.cards[i], cardHolder);
            card.cardHolderPosition = cardHolder;
            usableCards.Add(card);
        }

        data = new List<PlayerData>();
        roundT.text = numberOfRound.ToString();
    }

    [PunRPC]
    void ReceivePlayerPositionData(int index, string username, string uid)
    {
        if (uid.Equals(PhotonNetwork.LocalPlayer.UserId)) myIndex = index;
        data.Add(new PlayerData(index, username, uid));
        Debug.Log($"Position data: {index}. {username} count{data.Count}, mIndex:{myIndex}");
        if (data.Count == 4)
        {
            for (int i = myIndex; i < data.Count + myIndex; i++)
            {
                PlayerData d = data[i % data.Count];
                players[i - myIndex].Init(d.uid, d.username, d.index);
            }

            for (int i = 0; i < data.Count; i++)
            {
                scoreUi[i].usernameT.text = data[i].username;
                scoreUi[i].scoreT.text = "0";
                scoreUi[i].bagT.text = "0";
            }

            LoadingPanel.self.Show("Getting cards data");
        }
    }

    [PunRPC]
    void ReceivePlayerCardData(string uid, int[] ids)
    {
        if (uid.Equals(PhotonNetwork.LocalPlayer.UserId))
        {
            GetSelf().AddCardData(ids);
            cardHolder.gameObject.SetActive(false);
            Invoke(nameof(ShowBidPopup), so.bidPopupDelay);
            LoadingPanel.self.Hide();
        }
    }

    void ShowBidPopup()
    {
        bidTimerFill.value = 1;
        bidPanel.SetActive(true);
        bidTimerTween?.Kill();
        bidTimerTween = bidTimerFill.DOValue(0, so.maxBidTime).SetEase(Ease.Linear).OnComplete(() =>
        {
            bidPanel.SetActive(false);
            Btn_Bit(0);
        });
    }

    public void Btn_Bit(int amount)
    {
        bidPanel.SetActive(false);
        bidTimerTween?.Kill();
        photonView.RPC(nameof(ReceiveBidAmount), RpcTarget.All, PhotonNetwork.LocalPlayer.UserId, amount);

        if (PhotonNetwork.IsMasterClient)
        {
            currentTurnIndex = GetSelf().myIndex;
            GetSelf().IsMyTurn(GetSelf().myIndex);
            photonView.RPC(nameof(UpdateTurnIndex), RpcTarget.All, currentTurnIndex);
        }
    }

    public void PushTurn(string uid, int cardID)
    {
        GetSelf().turnDisabler.SetActive(true);
        GetSelf().StopIndicator();
        photonView.RPC(nameof(ReceiveTurnData), RpcTarget.All, uid, cardID);
    }

    [PunRPC]
    void ReceiveTurnData(string uid, int cardID)
    {
        Debug.Log($"{GetPlayer(uid).username} played {GetCardByID(cardID).Rank} {GetCardByID(cardID).Suit}");
        ShowMsg($"{GetPlayer(uid).username} played {GetCardByID(cardID).Rank} {GetCardByID(cardID).Suit}");
        GetPlayer(uid).StopIndicator();
        Card c = GetCardByID(cardID).RevealCard();
        c.transform.parent = turnCardHolder;
        c.gameObject.SetActive(true);
        c.GetComponent<Button>().interactable = true;
        
        cardsOnDeck.Add(GetPlayer(uid), c);

        if (PhotonNetwork.IsMasterClient)
        {            
            Debug.Log("cards on deck: " + cardsOnDeck.Count);
            if (cardsOnDeck.Count == 4)
            {
                totalTrickCount++;

                //get trick winner
                SandboxPlayerData leadingPlayer = null;
                Card leadingCard = null;
                foreach (var entry in cardsOnDeck)
                {
                    SandboxPlayerData player = entry.Key;
                    Card card = entry.Value;
                    if (leadingCard == null)
                    {
                        leadingCard = card;
                        leadingPlayer = player;
                    }
                    else
                    {
                        if (card.Suit == leadingCard.Suit)
                        {
                            if (card.Rank > leadingCard.Rank)
                            {
                                leadingCard = card;
                                leadingPlayer = player;
                            }
                        }
                        else if (card.Suit == Suit.Spades)
                        {
                            leadingCard = card;
                            leadingPlayer = player;
                        }
                    }
                }
                if (trickWinners.ContainsKey(leadingPlayer)) trickWinners[leadingPlayer]++;
                else trickWinners[leadingPlayer] = 1;

                photonView.RPC(nameof(UpdateScore), RpcTarget.All, leadingPlayer.uid);

                if (totalTrickCount >= 13)
                {
                    //game end
                    //show score and continue to next round
                    PushScoreData();                 
                }
                else
                {
                    Waiter.Wait(so.delayBeforeNewTurn, () =>
                    {
                        photonView.RPC(nameof(NewTurn), RpcTarget.All, leadingPlayer.myIndex);
                    });
                }
            }
            else photonView.RPC(nameof(UpdateTurnIndex), RpcTarget.All, currentTurnIndex);
        }
    }

    [PunRPC]
    void UpdateScore(string uid)
    {
        Debug.Log("winner: " + GetPlayer(uid).username);
        ShowMsg(GetPlayer(uid).username + " won the trick");
        GetPlayer(uid).UpdateScore();        
    }

    void StartNewGame()
    {
        cardsOnDeck.Clear();
        turnCardHolder.KillAllChild();
        cardHolder.KillAllChild();

        totalTrickCount = 0;
        for (int i = 0; i < usableCards.Count; i++)
        {
            if (usableCards[i] != null) Destroy(usableCards[i]);
        }
        usableCards.Clear();
        for (int i = 0; i < cardSO.cards.Length; i++)
        {
            Card card = Instantiate(cardSO.cards[i], cardHolder);
            card.cardHolderPosition = cardHolder;
            usableCards.Add(card);
        }

        for (int i = 0; i < players.Length; i++)
        {
            players[i].ResetForNewGame();
        }

        numberOfRound++;
        roundT.text = numberOfRound.ToString();

        if (PhotonNetwork.IsMasterClient && !isGameOver) Invoke(nameof(PushCardData), 1f);
    }

    [PunRPC]
    void NewTurn(int index)
    {
        Debug.Log("new turn");
        cardsOnDeck.Clear();
        turnCardHolder.KillAllChild();
        currentTurnIndex = index;
        UpdateTurnIndex(index);
    }

    [PunRPC]
    void UpdateTurnIndex(int index)
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].IsMyTurn(index);
        }        
        currentTurnIndex = (currentTurnIndex + 1) % players.Length;
    }

    [PunRPC]
    void ReceiveBidAmount(string uid, int amount)
    {
        Debug.Log($"{GetPlayer(uid).username} bid: {amount}");
        ShowMsg($"{GetPlayer(uid).username} bid: {amount}");
        playerBids[GetPlayer(uid)] = amount;
        GetPlayer(uid).BidSelected(amount);        
    }

    void PushScoreData()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        ScoreDataJson data = calculation.GetPlayerScore(playerBids, trickWinners);
        
        string json = JsonUtility.ToJson(data);

        var x = calculation.CheckForWinner();
        if (x == null)
        {
            isGameOver = false;
            photonView.RPC(nameof(ReceiveScoreData), RpcTarget.All, json);
        }
        else
        {
            isGameOver = true;
            photonView.RPC(nameof(ReceiveScoreData), RpcTarget.All, json);
            photonView.RPC(nameof(ShowGameOverPopup), RpcTarget.All, JsonUtility.ToJson(x));           
        }
    }

    [PunRPC]
    void ReceiveScoreData(string _data)
    {
        ScoreDataJson data = JsonUtility.FromJson<ScoreDataJson>(_data);
        for (int i = 0; i < data.scoreData.Count; i++)
        {
            scoreUi[i].usernameT.text = GetPlayer(data.scoreData[i].uid).username;
            scoreUi[i].scoreT.text = data.scoreData[i].score.ToString();
            scoreUi[i].bagT.text = data.scoreData[i].bagCount.ToString();
        }

        Waiter.Wait(so.newGameStartDelay, () =>
        {
            StartNewGame();
        });
    }

    [PunRPC]
    void ShowGameOverPopup(string _data)
    {
        ScoreDataJson data = JsonUtility.FromJson<ScoreDataJson>(_data);
        for (int i = 0; i < data.scoreData.Count; i++)
        {
            popupScoreUi[i].usernameT.text = GetPlayer(data.scoreData[i].uid).username;
            popupScoreUi[i].scoreT.text = data.scoreData[i].score.ToString();
            popupScoreUi[i].bagT.text = data.scoreData[i].bagCount.ToString();
        }

        winnerNameT.text = GetPlayer(data.scoreData[0].uid).username;
        winnerScoreT.text = data.scoreData[0].score.ToString();
        
        int winningAmount = ((PhotonNetwork.CurrentRoom.PlayerCount - 1) * GlobalData.roomEntryAmount);
        winnerTotalScoreT.text = data.scoreData[0].score.ToString();

        GameManager.Instance.User.TotalCurrency += winningAmount;
        BackendController.Instance.UserDatabase.UpdateUserData();

        popupParent.SetActive(true);
        popup1.SetActive(true);
        winnerPanelTimer.value = 1;
        winnerPanelTimer.DOValue(0, so.winnerPanelTime).SetEase(Ease.Linear).OnComplete(() =>
        {
            winnerPanelTimer.value = 1;
            popup2.SetActive(true);
            popup1.SetActive(false);
            winnerPanelTimer.DOValue(0, so.winnerPanelTime).SetEase(Ease.Linear).OnComplete(() =>
            {
                //go to menu scene
                if (PhotonNetwork.InRoom) PhotonNetwork.LeaveRoom();
                SceneManager.LoadScene(0);
            });
        });
    }


    public SandboxPlayerData GetPlayer(string uid)
    {
        SandboxPlayerData p = players[0];
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].uid.Equals(uid))
            {
                p = players[i];
                break;
            }
        }
        return p;
    }

    public SandboxPlayerData GetSelf() => GetPlayer(PhotonNetwork.LocalPlayer.UserId);

    public Card GetCardByID(int id)
    {
        return usableCards[id];
    }

    public Card GetLeadingCard()
    {
        if (cardsOnDeck.Count == 0)
        {
            return null;
        }
        else return cardsOnDeck.Values.First();
    }

    public void Btn_MenuScene()
    {
        if (PhotonNetwork.InRoom) PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }

    void ShowMsg(string msg)
    {
        msgT.text = msg;
    }

    [System.Serializable]
    class PlayerData
    {
        public int index;
        public string username;
        public string uid;

        public PlayerData(int _index, string _username, string _uid)
        {
            index = _index;
            username = _username;
            uid = _uid;
        }
    }

    [System.Serializable]
    public class ScoreUI
    {
        public TMP_Text usernameT, scoreT, bagT;
    }

    [System.Serializable]
    public class ScoreData
    {
        public string uid;
        public int score;
        public int bagCount;

        public ScoreData(string _uid, int _score, int _bagCount)
        {
            uid = _uid;
            score = _score;
            bagCount = _bagCount;
        }
    }

    [System.Serializable]
    public class ScoreDataJson
    {
        public List<ScoreData> scoreData = new List<ScoreData>();
    }
}
