using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class MatchmakingPanel : MonoBehaviourPunCallbacks
{
    public int gameSceneIndex;
    public GameObject[] playerAvatars;
    public GameObject t1, t2;
    public TMP_Text roomCodeText;
    public TMP_Text infoText;

    private void Start()
    {
        t1.SetActive(true);
        t2.SetActive(false);
        infoText.text = "Please wait for players to join";
        playerAvatars[0].SetActive(true);
        for (int i = 1; i < playerAvatars.Length; i++)
        {
            playerAvatars[i].SetActive(false);
        }

        ControllerPhoton.self.OnMatchmakingEnd.RemoveListener(OnMatchmakingEnd);
        ControllerPhoton.self.OnMatchmakingEnd.AddListener(OnMatchmakingEnd);
    }

    void OnMatchmakingEnd()
    {
        if (PhotonNetwork.IsMasterClient)
        GetComponent<PhotonView>().RPC(nameof(OnHouseFull), RpcTarget.All);
    }

    [PunRPC]
    void OnHouseFull()
    {
        t1.SetActive(false);
        t2.SetActive(true);
        infoText.text = "Match is starting.";
        CancelInvoke();
        Invoke(nameof(LoadLevel), 1f);
    }

    void LoadLevel()
    {
        PhotonNetwork.LoadLevel(gameSceneIndex);
    }

    
    public override void OnJoinedRoom()
    {
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            playerAvatars[i].SetActive(true);
        }

        roomCodeText.text = GlobalData.roomCode;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        for (int i = 0; i < playerAvatars.Length; i++)
        {
            if (!playerAvatars[i].activeInHierarchy)
            {
                playerAvatars[i].SetActive(true);
                break;
            }
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        for (int i = playerAvatars.Length - 1; i >= 0; i--)
        {
            if (playerAvatars[i].activeInHierarchy)
            {
                playerAvatars[i].SetActive(false);
                break;
            }
        }
    }

    public override void OnLeftRoom()
    {
        Start();
    }

    public void Btn_LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        LoadingPanel.self.Show();
        HomeUIController.Instance.ShowMainPanel();
    }
}
