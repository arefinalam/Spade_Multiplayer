using TMPro;
using UnityEngine;

public class LoadingPanel : MonoBehaviour
{
    public static LoadingPanel self;
    [SerializeField] TMP_Text text;

    private void Awake()
    {
        if (self == null) self = this;
        else Destroy(gameObject);
        Hide();
    }

    public void Hide() => gameObject.SetActive(false);
    public void Show(string msg = "")
    {
        text.text = msg.Equals("") ? "Loading..." : msg;
        gameObject.SetActive(true);
    }
}
