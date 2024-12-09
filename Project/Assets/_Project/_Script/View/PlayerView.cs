using TMPro;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    public TMP_Text nicknameText;
    public int index;
    public string uid;
    public string nickname;

    public PlayerView Init(int _index, string _uid, string _nickname)
    {
        index = _index;
        uid = _uid;
        nickname = _nickname;
        nicknameText.text = nickname;
        return this;
    }
}
