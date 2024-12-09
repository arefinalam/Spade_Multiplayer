using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PrivateRoomPanelView : MonoBehaviour
{
    [SerializeField] private Button createRoomButton;
    [SerializeField] private TMP_InputField roomIdInput;
    [SerializeField] private Button joinRoomButton;
    // Start is called before the first frame update
    void Start()
    {
        createRoomButton.onClick.AddListener(CreatePrivateRoom);
        joinRoomButton.onClick.AddListener(JoinPrivateRoom);
    }

    private void CreatePrivateRoom()
    {
        ControllerPhoton.self.JoinRoom(GlobalData.roomEntryAmount);
        HomeUIController.Instance.ShowMatchmakingPanel();
    }

    private void JoinPrivateRoom()
    {
        if (roomIdInput.text.Length != 6)
        {
            Debug.LogError("Room id must be 6 characters");
            return;
        }
        LoadingPanel.self.Show();
        ControllerPhoton.self.JoinRoomWithCode(roomIdInput.text, OnRoomFound, OnRoomNotFound);
    }

    void OnRoomFound()
    {
        HomeUIController.Instance.ShowMatchmakingPanel();
    }

    void OnRoomNotFound()
    {
        LoadingPanel.self.Hide();
    }
}
