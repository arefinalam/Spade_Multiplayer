using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TempTest : MonoBehaviour
{
    [ButtonLUFI]
    void LoadScene()
    {
        PhotonNetwork.LoadLevel(1);
    }
}
