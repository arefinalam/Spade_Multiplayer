using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public string  userId;
    public string  userName;
    public int tableIndex;
    public Transform handTransform; 
    public GameObject cardDisablePanel;
    public GameObject bidPopupPrefab;
    public TMP_Text currentBidText;
    public TMP_Text trickWonText;
    public TMP_Text usernameText;

    private List<Card> playerCards = new List<Card>();
    
    private string TableTagString = "SpadeTable";
    private Transform tableTransform;

    private int currentScore = 0;
    public int currentBid = 0;
    private int totalTrickWon = 0;

    private void Start()
    {
        playerCards ??= new List<Card>();
        playerCards.Clear();
        tableTransform = GameObject.FindGameObjectWithTag(TableTagString).transform;
        SetTotalTrickWon(totalTrickWon);
        SetCurrentBid(currentBid);
    }

    public void Setup(string _name, string _uid, int index)
    {
        userName = _name;
        userId = _uid;
        tableIndex=index;
        //update text here
        usernameText.text = userName;
    }

    public void AddCard(Card card)
    {
        playerCards.Add(card);
        card.transform.parent = handTransform;
    }

    public void ReceiveCards(int[] cardIDs)
    {
        Debug.Log("cardIDs: " + cardIDs.Length + " cards: " + playerCards.Count);
        for (int i = 0; i < cardIDs.Length; i++)
        {
            Card c = GameplayManager.Instance.GetCardByID(cardIDs[i]);
            playerCards[i].UpdateCard(c.Suit, c.Rank, c.CardNumber, c.cardSprite, c.cardFlipSprite);
        }

        
        foreach (Card card in playerCards)
        {
            // Animate the card to the hand position using DoTween
            card.transform.DOMove(handTransform.position, 0.5f).SetEase(Ease.InOutQuad);
            card.transform.SetParent(handTransform, false);
            card.gameObject.SetActive(true);
            card.ToggleSpirte(false);
        }
    }

    public void FakeMove(int cardID)
    {
        Card c = playerCards[0];
        Card _c = GameplayManager.Instance.GetCardByID(cardID);
        c.UpdateCard(_c.Suit, _c.Rank, cardID, _c.cardSprite, _c.cardFlipSprite);
        c.RevealCard();
        c.transform.SetParent(GameplayManager.Instance.CardManager().cardHolderPanel, false);
        c.transform.SetAsLastSibling();
        c.GetComponent<Button>().interactable = true;
        c.transform.DOLocalRotate(GameplayManager.Instance.CardManager().cardHolderPanel.transform.localEulerAngles, 0.5f);
        c.transform.DOLocalMove(GameplayManager.Instance.CardManager().cardHolderPanel.transform.position, 1.5f).OnComplete(() =>
        {
            playerCards.RemoveAt(0);
        });
    }

    public void ResetCard()
    {
        playerCards.Clear();
    }

    public IEnumerator ShowBidPopup()
    {
        GameObject popup = Instantiate(bidPopupPrefab, tableTransform.transform);
        BidPopup bidPopup = popup.GetComponent<BidPopup>();
        bidPopup.Setup(this, GameplayManager.Instance.gameData.maxBidTime);

        yield return new WaitForSeconds(GameplayManager.Instance.gameData.maxBidTime);

        if(popup != null)
        {
            Destroy(bidPopup);
        }
    }

    public void OnBidSubmitted(int bid)
    {
        ToggleCardDisablePanel(true);
        GameplayManager.Instance.BidController().CollectBid(this, bid);
        SetCurrentBid(bid);
    }

    public void ToggleCardDisablePanel(bool isEnable)
    {
        cardDisablePanel.SetActive(isEnable);
        ResetPanelWidth();
    }

    private void ResetPanelWidth()
    {
        // Set the width of the RectTransform
        Vector2 sizeDelta = handTransform.GetComponent<RectTransform>().sizeDelta;
        cardDisablePanel.GetComponent<RectTransform>().sizeDelta = sizeDelta;
    }


    public void PlayTrick(int roundIndex)
    {
        if (tableIndex != roundIndex)
        {
            ToggleCardDisablePanel(true);
            return;
        }
            

        //ToggleCardDisablePanel(true);
        cardDisablePanel.SetActive(roundIndex == tableIndex ? false : true);
        Card card  = GameplayManager.Instance.TrickManager().CardsEligibleToPlay();
        CheckCardsEligibleToPlay(card);
    }

    private void CheckCardsEligibleToPlay(Card leadingCard)
    {
        if(leadingCard == null)
        {
            foreach(Card card in playerCards)
            {
                card.ToggleButtonInteraction(true);
            }
        }
        else
        {
            // Check if the player has any card of the leading suit
            bool hasLeadingSuit = false;
            foreach (Card item in playerCards)
            {
                if (item.Suit == leadingCard.Suit)
                {
                    hasLeadingSuit = true;
                    break;
                }
            }

            if(hasLeadingSuit)
            {
                foreach (Card item in playerCards)
                {
                    bool eligible = item.Suit == leadingCard.Suit;
                    item.ToggleButtonInteraction(eligible);
                }
            }
            else
            {
                foreach (Card item in playerCards)
                {
                    item.ToggleButtonInteraction(true);
                }
            }
        }
    }

    public void SetCurrentBid(int bid)
    {
        currentBid = bid;
        currentBidText.text = bid.ToString();
    }
    public void SetTotalTrickWon(int trickWon)
    {
        totalTrickWon = trickWon;
        trickWonText.text = trickWon.ToString();
    }

    public void SetScore(int score)
    {
        currentScore = score;
    }

    public void ResetData()
    {
        totalTrickWon = 0;
        currentBid = 0;
        SetCurrentBid(currentBid);
        SetTotalTrickWon(totalTrickWon);
    }
}
