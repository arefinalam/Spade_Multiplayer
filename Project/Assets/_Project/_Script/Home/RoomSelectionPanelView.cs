using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomSelectionPanelView : MonoBehaviour
{
    [SerializeField] private Button randomRoomButton;
    [SerializeField] private Button privateRoomButton;
    // Start is called before the first frame update
    void Start()
    {
        randomRoomButton.onClick.AddListener(OnRandomRoomButtonCLick);
        privateRoomButton.onClick.AddListener(OnPrivateRoomButtonCLick);
    }

    // Update is called once per frame
    private void OnRandomRoomButtonCLick()
    {
        GlobalData.isPrivateRoom = false;
        ControllerPhoton.self.JoinRoom(GlobalData.roomEntryAmount);
        HomeUIController.Instance.ShowMatchmakingPanel();
    }

    private void OnPrivateRoomButtonCLick()
    {
        GlobalData.isPrivateRoom = true;
        HomeUIController.Instance.ShowPrivateRoomPanel();
    }
}
