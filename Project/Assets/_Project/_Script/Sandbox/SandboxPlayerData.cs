using DG.Tweening;
using Photon.Pun;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SandboxPlayerData : MonoBehaviour
{
    public TMP_Text bidTotalT, bidWinT, nameT;
    public string uid;
    public string username;
    public int myIndex;
    public Transform hand;
    public int selectedBit;
    public GameObject turnDisabler;
    public GameObject indicator;
    public List<Card> myCard;
    public List<Card> eligibleCards;

    int score;
    Tween timerTween;

    public void Init(string _uid, string _username, int index)
    {
        Debug.Log("player data init " + _username);
        score = 0;
        uid = _uid;
        username = _username;
        myIndex = index;
        nameT.text = username;
        turnDisabler.SetActive(true);
    }

    public void AddCardData(int[] cards)
    {
        myCard ??= new List<Card>(cards.Length);
        myCard.Clear();

        for (int i = 0; i < cards.Length; i++)
        {
            Card c = SandboxGameplay.self.GetCardByID(cards[i]);
            c.transform.parent = hand;
            c.RevealCard();
            c.SetOwner(this);
            myCard.Add(c);
        }
    }

    public void RemoveCard(Card card)
    {
        myCard.Remove(card);
    }

    public void IsMyTurn(int index)
    {
        turnDisabler.SetActive(myIndex != index);
        indicator.SetActive(myIndex == index);

        if (myIndex != index) return;

        indicator.GetComponent<Animator>().enabled = false;
        Color c = indicator.GetComponent<Image>().color;
        c.a = 1;
        indicator.GetComponent<Image>().color = c;
        indicator.GetComponent<Image>().fillAmount = 1;
        timerTween?.Kill();
        timerTween = indicator.GetComponent<Image>().DOFillAmount(0, SandboxGameplay.self.so.eachTurnDuration).SetEase(Ease.Linear).OnComplete(() =>
        {
            if (PhotonNetwork.LocalPlayer.UserId.Equals(uid))
            {
                //play a default card
                turnDisabler.SetActive(false);
                if (eligibleCards.Count > 0) eligibleCards[0].OnClick();
            }
        });

        CancelInvoke();
        Invoke(nameof(Anim), SandboxGameplay.self.so.turnAlertDelay);

        ShowEligibleCards(SandboxGameplay.self.GetLeadingCard());
    }

    void Anim()
    {
        indicator.GetComponent<Animator>().enabled = true;
    }

    public void StopIndicator()
    {
        CancelInvoke();
        timerTween?.Kill();
    }

    public void BidSelected(int amount)
    {
        selectedBit = amount;
        bidTotalT.text = amount.ToString();
    }

    public void UpdateScore()
    {        
        score++;
        bidWinT.text = score.ToString();
    }

    public void ResetForNewGame()
    {
        IsMyTurn(-1);
        StopIndicator();
        score = 0;
        selectedBit = 0;
        bidTotalT.text = selectedBit.ToString();
        bidWinT.text = score.ToString();
    }

    void ShowEligibleCards(Card leadingCard)
    {
        foreach (Card card in myCard)
        {
            card.ToggleButtonInteraction(false);
        }

        eligibleCards.Clear();
        if (leadingCard == null)
        {
            foreach (Card card in myCard)
            {
                card.ToggleButtonInteraction(true);
                eligibleCards.Add(card);
            }
        }
        else
        {
            foreach (Card card in myCard)
            {
                if (card.Suit == leadingCard.Suit)
                {
                    card.ToggleButtonInteraction(true);
                    eligibleCards.Add(card);
                }
            }

            if (eligibleCards.Count == 0)
            {
                foreach (Card card in myCard)
                {
                    card.ToggleButtonInteraction(true);
                    eligibleCards.Add(card);
                }
            }
        }
    }
}
