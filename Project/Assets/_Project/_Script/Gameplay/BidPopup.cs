using UnityEngine;
using UnityEngine.UI;

public class BidPopup : MonoBehaviour
{
    //public TextMeshProUGUI playerNameText;
    public Button[] bidOptionButtons; // Array of bid option buttons
    public Button submitButton;
    private PlayerController player;
    private int selectedBid = -1; // Default bid is 0
    public bool BidSubmitted { get; private set; } = false; // Property to track if bid is submitted
    private float currentTimer;

    private void Awake()
    {
        submitButton.onClick.AddListener(OnSubmitButtonClicked); 
    }

    public void Setup(PlayerController player, float maxBidTime)
    {
        this.player = player;
        // Add click listeners to bid option buttons
        for (int i = 0; i < bidOptionButtons.Length; i++)
        {
            int bidValue = i; // Capture bid value for lambda expression
            bidOptionButtons[i].onClick.AddListener(() => OnBidOptionClicked(bidValue));
        }
        bidOptionButtons[0].Select();
        
        //playerNameText.text = player.name;
        Invoke(nameof(DefaultBid), maxBidTime); // Set default bid after maxBidTime
    }

    private void DefaultBid()
    {
        if (selectedBid == -1)
        {
            selectedBid = 0;
            LogManager.Instance.ConsoleLog($"Player {player.name} did not submit a bid in time. Defaulting to 0 bid.");
        }
        OnSubmitButtonClicked();
    }

    private void OnBidOptionClicked(int bid)
    {
        selectedBid = bid;
        LogManager.Instance.ConsoleLog($"Bid option {bid} selected by {player.name}");
    }

    private void OnSubmitButtonClicked()
    {
        GameplayManager.Instance.Photon().PushBidAmount(selectedBid);
        BidSubmitted = true; // Mark bid as submitted
        LogManager.Instance.ConsoleLog($"Player {player.userName} submitted a bid of {selectedBid}");
        GameplayManager.Instance.UIManager().ReceiveGameplayMessage($"Player {player.userName} submitted a bid of {selectedBid}");
        Destroy(this.gameObject);
    }
}
