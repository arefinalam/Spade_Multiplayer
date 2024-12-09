using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using System;
using UnityEngine.Events;

public class ControllerPhoton : MonoBehaviourPunCallbacks
{
    public static ControllerPhoton self;
    
    public List<Player> playersInRoom = new List<Player>();
    public UnityEvent OnServerConnect;
    public UnityEvent OnRoomJoin;
    public UnityEvent OnMatchmakingEnd;

    List<RoomInfo> avaliableRoomInfos;

    private void Awake()
    {
        if (self == null) self = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void Init(string uid, string displayName)
    {
        Debug.Log("is connected: " + PhotonNetwork.IsConnected);
        if (PhotonNetwork.IsConnected) return;

        LoadingPanel.self.Show("Connecting to server");
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = Application.version;
        PhotonNetwork.AuthValues = new AuthenticationValues(uid);
        PhotonNetwork.NickName = displayName;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Log(PhotonNetwork.NickName + " connected to server.");
        PhotonNetwork.JoinLobby();
        LoadingPanel.self.Hide();
        OnServerConnect?.Invoke();
    }


    public void JoinOrCreateRoom()
    {
        Log("Room searching...");
        if (PhotonNetwork.IsConnected)
        {
            Hashtable hash = new Hashtable
            {
                { nameof(GlobalData.roomEntryAmount), GlobalData.roomEntryAmount },
            };

            //try to join random room
            PhotonNetwork.JoinRandomRoom(hash, 0);
        }
        else
        {
            Log("Not connected to Server. Trying again to connect.");
            Init(GlobalData.uid, GlobalData.nickname);
        }
    }

    public override void OnJoinedRoom()
    {
        playersInRoom.Clear();
        AddOtherPlayerData(PhotonNetwork.LocalPlayer);
        LoadingPanel.self.Hide();
        FindObjectOfType<HomeUIController>().ShowMatchmakingPanel();
        Log("Joined room: " + PhotonNetwork.CurrentRoom.Name);
        GlobalData.matchID = PhotonNetwork.CurrentRoom.Name;
        GlobalData.roomCode = GlobalData.matchID.Split('>')[0];

        //self data
        //FindObjectOfType<HomeUIController>().matchmakingPanel.GetComponent<MatchmakingPanel>().playersText[0].text = PhotonNetwork.LocalPlayer.NickName;

        //info of players in room
        if (PhotonNetwork.InRoom)
        {
            Dictionary<int, Player> _playersInRoom = PhotonNetwork.CurrentRoom.Players;
            foreach (var _player in _playersInRoom)
            {
                AddOtherPlayerData(_player.Value);
            }
        }
        OnRoomJoin?.Invoke();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Log("No room available, creating a new room...");

        // Create a new room if join random failed
        CreateNewRoom();
    }

    public override void OnLeftRoom()
    {
        Log("You left the room");
        GlobalData.matchID = "";        
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddOtherPlayerData(newPlayer);        
        CheckMatchmaking();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        int index = playersInRoom.FindIndex(x=> x.UserId.Equals(otherPlayer.UserId));

        if (index != -1)
        {
            Log(otherPlayer.NickName + " left the room");
            //Destroy(playersInRoom[index].gameObject);
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        avaliableRoomInfos ??= new List<RoomInfo>();
        avaliableRoomInfos.Clear();
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].IsOpen) avaliableRoomInfos.Add(roomList[i]);
        }
        Debug.Log("total open room: " + avaliableRoomInfos.Count);
    }

    public void JoinRoomWithCode(string roomCode, Action OnRoomFound = null, Action OnRoomNotFound = null)
    {
        bool b = false;
        for (int i = 0; i < avaliableRoomInfos.Count; i++)
        {
            Debug.Log("roomName: " + avaliableRoomInfos[i].Name + " code: " + roomCode + " => " + avaliableRoomInfos[i].Name.StartsWith(roomCode));
            if (avaliableRoomInfos[i].Name.StartsWith(roomCode))
            {
                PhotonNetwork.JoinRoom(avaliableRoomInfos[i].Name);
                OnRoomFound?.Invoke();
                b = true;
                break;
            }
        }
        if (!b)
        {
            Log("no avaliable room with code: " + roomCode);
            OnRoomNotFound?.Invoke();
        }
    }

    public void LeaveRoom()
    {
        if (PhotonNetwork.InRoom) PhotonNetwork.LeaveRoom();
        else Log("Not currently in a room");

        playersInRoom.Clear();
        GlobalData.matchID = "";
        GlobalData.roomEntryAmount = 0;
    }

    void AddOtherPlayerData(Player p) 
    {
        //if (p.UserId.Equals(PhotonNetwork.LocalPlayer.UserId)) return;
        
        int index = playersInRoom.FindIndex(x=> x.UserId.Equals(p.UserId));

        if (index == -1)
        {            
            //playersInRoom.Add(Instantiate(playerView, playerViewHolder).Init(0, p.UserId, p.NickName));
            playersInRoom.Add(p);
            Log("Player found in room: " + p.NickName);
        }
    }

    void CreateNewRoom()
    {
        RoomOptions roomOptions = new RoomOptions
        {
            IsOpen = true,
            IsVisible = !GlobalData.isPrivateRoom,
            MaxPlayers = 4,
            PublishUserId = true,
            CustomRoomProperties = new Hashtable
                {
                    { nameof(GlobalData.roomEntryAmount), GlobalData.roomEntryAmount },
                    { nameof(GlobalData.roomCode), GlobalData.roomCode },
                },
            CustomRoomPropertiesForLobby = new string[] { nameof(GlobalData.roomEntryAmount), nameof(GlobalData.roomCode) }
        };

        PhotonNetwork.CreateRoom(GetRoomID(), roomOptions);
    }

    void CheckMatchmaking()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            OnMatchmakingEnd?.Invoke();
        }
    }

    string GetRoomID()
    {
        DateTime time = DateTime.Now;
        string _name = PhotonNetwork.NickName.Length > 5 ? PhotonNetwork.NickName.Substring(0, 5) : PhotonNetwork.NickName;

        GlobalData.roomCode = UnityEngine.Random.Range(100000, 1000000).ToString();

        string id = $"{GlobalData.roomCode}>{time.Month}/{time.Day}/{time.Hour}:{time.Minute}-{GlobalData.roomEntryAmount}-{_name}-{UnityEngine.Random.Range(1000, 10000)}";
        Log("Generated roomID: " + id);
        return id;
    }

    public void JoinRoom(int roomEnteryAmount)
    {
        LoadingPanel.self.Show();
        GlobalData.roomEntryAmount = roomEnteryAmount;
        JoinOrCreateRoom();
    }

    public void Btn_ChangeRoomVisibility(bool isPrivate)
    {
        GlobalData.isPrivateRoom = isPrivate;
    }

    void Log(string msg)
    {
        //Debug.Log(msg);
        LogManager.Instance.ConsoleLog(msg);
    }
}
