using Photon.Pun;
using System.Collections;
using UnityEngine;

public class PlayerCountHelper : MonoBehaviour
{
    [SerializeField] int updateDelay = 3;

    private void Start()
    {
        StartWork();
        ControllerPhoton.self.OnServerConnect.RemoveListener(StartWork);
        ControllerPhoton.self.OnServerConnect.AddListener(StartWork);
    }

    void StartWork()
    {
        StopAllCoroutines();
        if (PhotonNetwork.IsConnected) StartCoroutine(UpdateData());
    }

    void OnDisable()
    {
        ControllerPhoton.self.OnServerConnect.RemoveListener(StartWork);
        StopAllCoroutines();
    }

    void OnDestroy()
    {
        ControllerPhoton.self.OnServerConnect.RemoveListener(StartWork);
        StopAllCoroutines();
    }


    IEnumerator UpdateData()
    {
        while (PhotonNetwork.IsConnected)
        {
            HomeUIController.Instance.onlinePlayerCountText.text = PhotonNetwork.CountOfPlayers.ToString();
            yield return new WaitForSecondsRealtime(updateDelay);
        }
    }
}
