using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using static GameplayManager;

[RequireComponent(typeof(PhotonView))]
public class GameplayPhoton : MonoBehaviourPun
{
    List<PlayerData> data = new List<PlayerData>(4);
    private int myIndex;
    int currentTurnIndex;
    public void PushPlayerData()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < ControllerPhoton.self.playersInRoom.Count; i++)
            {
                Player p = ControllerPhoton.self.playersInRoom[i];
                photonView.RPC(nameof(ReceivePlayerPositionData), RpcTarget.All, i, p.NickName, p.UserId);
            }

            GameplayManager.Instance.StartSuffleAndDistributeCards();
        }
        else GameplayManager.Instance.CardManager().DistributeCards();
    }

    [PunRPC]
    public void ReceivePlayerPositionData(int index, string username, string uid)
    {
        if (uid.Equals(PhotonNetwork.LocalPlayer.UserId)) myIndex = index;
        data.Add(new PlayerData(index, username, uid));
        Debug.Log($"Position data: {index}. {username} count{data.Count}, mIndex:{myIndex}");
        if (data.Count == 4)
        {
            for (int i = myIndex; i < data.Count + myIndex; i++)
            {
                PlayerData d = data[i % data.Count];
                GameplayManager.Instance.Players()[i - myIndex].Setup(d.username, d.uid, d.index);
            }
        }
    }

    public void PushCards(int[] playerCards, string uid)
    {
        GetComponent<PhotonView>().RPC(nameof(ReceiveCards), RpcTarget.All, uid, playerCards);
    }

    [PunRPC]
    void ReceiveCards(string uid, int[] cardID)
    {
        if (uid.Equals(PhotonNetwork.LocalPlayer.UserId))
        {
            GameplayManager.Instance.GetSelfPlayer().ReceiveCards(cardID);
            GameplayManager.Instance.CardDistributionDone();
        }    
    }

    public void PushBidAmount(int amount)
    {
        photonView.RPC(nameof(ReceiveBidAmount), RpcTarget.All, PhotonNetwork.LocalPlayer.UserId, amount);
    }
    [PunRPC]
    void ReceiveBidAmount(string uid, int amount)
    {
        GameplayManager.Instance.GetPlayer(uid).OnBidSubmitted(amount);
    }

    [PunRPC]
    public void UpdateTurnIndex(int index)
    {
        GameplayManager.Instance.GetSelfPlayer().PlayTrick(index);
        currentTurnIndex = (currentTurnIndex + 1) % GameplayManager.Instance.Players().Count;
        LogManager.Instance.ConsoleLog("Next player turn:" + GameplayManager.Instance.GetPlayerByTableIndex(currentTurnIndex).userName);
    }
    public void PushGameStarter(int tableIndex)
    {
        GameplayManager.Instance.UpdateStep();
        photonView.RPC(nameof(UpdateTurnIndex), RpcTarget.All, tableIndex);
    }

    public void PushTurn(string uid, int cardID)
    {
        GameplayManager.Instance.GetSelfPlayer().ToggleCardDisablePanel(true);
        photonView.RPC(nameof(ReceiveTurnData), RpcTarget.All, uid, cardID);
    }

    [PunRPC]
    public void ReceiveTurnData(string uid, int cardID)
    {
        //Play Animation
        GameplayManager.Instance.GetPlayer(uid).FakeMove(cardID);
        GameplayManager.Instance.TrickManager().TrickPlayed(GameplayManager.Instance.GetPlayer(uid), 
            GameplayManager.Instance.GetCardByID(cardID));
    }

    public void PushTrickWinner(string uid)
    {
        photonView.RPC(nameof(ReceiveTrickWinner), RpcTarget.All, uid);
    }

    [PunRPC]
    void ReceiveTrickWinner(string uid)
    {
        GameplayManager.Instance.RoundManager().SetTrickWinner(GameplayManager.Instance.GetPlayer(uid));
        GameplayManager.Instance.TrickManager().SetPlayerTrickStats();
    }

    public void NextPlayerTurn()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        photonView.RPC(nameof(UpdateTurnIndex), RpcTarget.All, currentTurnIndex);
    }

    public void PushResult(PlayerController player, int score)
    {
        photonView.RPC(nameof(ReceiveResult), RpcTarget.All, player.userId, score);
    }

    [PunRPC]
    void ReceiveResult(string uid, int score)
    {
        //Update UI

        
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
}


