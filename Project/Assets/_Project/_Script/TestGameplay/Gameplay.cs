using Helper.Waiter;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Gameplay : MonoBehaviour
{
    public PlayerView[] players;
    public TMP_Text[] scoreTexts;
    public TMP_Text[] pointText;
    public GameObject turnPlayBtn;

    int myIndex = 0;
    int numberOfTurn = 0;
    List<Turn> currentTurn;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //setup
            Dictionary<int, Player> _playersInRoom = PhotonNetwork.CurrentRoom.Players;

            int index = 0;
            foreach (var _p in _playersInRoom)
            {
                GetComponent<PhotonView>().RPC(nameof(SetupPlayers), RpcTarget.All, index, _p.Value.UserId, _p.Value.NickName);
                index++;
            }
        }

        turnPlayBtn.SetActive(PhotonNetwork.IsMasterClient);
    }

    [PunRPC]
    void SetupPlayers(int index, string uid, string nickname)
    {
        if (uid.Equals(PhotonNetwork.LocalPlayer.UserId)) myIndex = index;
        players[index].Init(index, uid, nickname);
    }

    public void Btn_PlayTurn()
    {
        int score = Random.Range(0, 10);
        GetComponent<PhotonView>().RPC(nameof(SyncTurn), RpcTarget.All, myIndex, score, numberOfTurn);
    }

    [PunRPC]
    void SyncTurn(int playerIndex, int score, int _numberOfTurn)
    {
        numberOfTurn = _numberOfTurn++;
        currentTurn ??= new List<Turn>();
        currentTurn.Add(new Turn { id = playerIndex, score = score });

        scoreTexts[playerIndex].text = score.ToString();        

        if (numberOfTurn == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                //decide winner
                Turn t = GetHighBet(currentTurn.ToArray());
                pointText[t.id].text += "0";

                Waiter.Wait(1.5f, () =>
                {
                    GetComponent<PhotonView>().RPC(nameof(NextRound), RpcTarget.All, t.id);
                });
            }
        }
        else if (playerIndex + 1 == myIndex)
        {
            //my turn
            turnPlayBtn.SetActive((playerIndex + 1) % PhotonNetwork.CurrentRoom.MaxPlayers == myIndex);
        }
    }

    [PunRPC]
    void NextRound(int index)
    {
        numberOfTurn = 0;
        currentTurn.Clear();
        for (int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
        {
            scoreTexts[i].text = "?";
        }
        turnPlayBtn.SetActive(index == myIndex);
    }

    Turn GetHighBet(Turn[] turns)
    {
        int max = 0;
        int maxIndex = 0;
        for (int i = 0; i < turns.Length; i++)
        {
            if (turns[i].score > max)
            {
                max = turns[i].score;
                maxIndex = i;
            }
        }        
        return turns[maxIndex];
    }


    [System.Serializable]
    public class Turn
    {
        public int id;
        public int score;
    }
}
