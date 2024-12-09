using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public Suit Suit;
    public Rank Rank;
    public int CardNumber;
    public Sprite cardSprite;
    public Sprite cardFlipSprite;

    private Button button;
    private Image cardImage;
    public Transform cardHolderPosition;
    
    public SandboxPlayerData playerData;

    private void Awake()
    {
        button = GetComponent<Button>();
        cardImage = GetComponent<Image>();
        ToggleSpirte(true);
        button.onClick.AddListener(OnClick);
    }

    public Card RevealCard()
    {
        gameObject.SetActive(true);
        cardImage.sprite = cardSprite;
        return this;
    }

    public void SetOwner(SandboxPlayerData playerData) => this.playerData = playerData;

    public void UpdateCard(Suit _suit, Rank _rank, int _cardNumber, Sprite _cardSprite, Sprite _cardFlipSprite)
    {
        Suit = _suit;
        Rank = _rank;
        CardNumber = _cardNumber;
        cardSprite = _cardSprite;
        cardFlipSprite = _cardFlipSprite;
    }

    public void OnClick()
    {
        gameObject.SetActive(false);
        if (playerData != null) playerData.RemoveCard(this);
        SandboxGameplay.self.GetSelf().turnDisabler.SetActive(true);
        SandboxGameplay.self.PushTurn(Photon.Pun.PhotonNetwork.LocalPlayer.UserId, CardNumber);
    }


    // Override ToString for debugging
    public override string ToString()
    {
        return $"{Rank} of {Suit}";
    }

    public void ToggleButtonInteraction(bool isInteractable)
    {
        button.interactable = isInteractable;
    }

    public void ToggleSpirte(bool isFlipped)
    {
        if (isFlipped)
        {
            cardImage.sprite = cardFlipSprite;
            ToggleButtonInteraction(false);
        }
        else
        {
            cardImage.sprite = cardSprite;
        }
    }
}
