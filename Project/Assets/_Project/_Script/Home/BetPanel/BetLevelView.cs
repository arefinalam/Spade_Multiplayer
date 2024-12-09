using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class BetLevelView : MonoBehaviour
{
    public TextMeshProUGUI levelNameText;
    public TextMeshProUGUI entryAmountText;
    public Button btn;
    public Image betImage;
    int roomEntryAmount;

    public void SetBetLevelDetails(string levelName, int entryAmount, Sprite betSprite)
    {
        roomEntryAmount = entryAmount;
        levelNameText.text = levelName;
        entryAmountText.text = entryAmount.ToString();
        betImage.sprite = betSprite;
        SetState(GameManager.Instance.User.TotalCurrency >= entryAmount);
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(BtnClick);
    }

    void BtnClick()
    {
        GlobalData.roomEntryAmount = roomEntryAmount;
        HomeUIController.Instance.ShowRoomSelectionPanel();
        GameManager.Instance.CurrentBetAmount = roomEntryAmount;
        LogManager.Instance.ConsoleLog("Starting game for bet level: " + levelNameText.text);
    }

    void SetState(bool isInteractable)
    {
        // update in the UI
        btn.interactable = isInteractable;
    }
}
