using Helper.Extension;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public CardSO cardsSO;
    public List<Card> usableCards;
    public Transform cardHolderPanel;
    public Transform sufflePoint;
    [SerializeField] private List<GameObject> cardObjects;
    private List<Card> suffledCard = new List<Card>();
    
    public void Initialize()
    {
        usableCards ??= new List<Card>();
        usableCards.Clear();
        suffledCard ??= new List<Card>();
        suffledCard.Clear();

        for (int i = 0; i < cardsSO.cards.Length; i++)
        {
            var x = Instantiate(cardsSO.cards[i], cardHolderPanel);
            x.cardHolderPosition = cardHolderPanel;
            usableCards.Add(x);
            suffledCard.Add(x);
        }

        /*foreach (GameObject cardObj in cardObjects)
        {
            cardObj.transform.position = sufflePoint.transform.position;
            cardObj.transform.parent = sufflePoint.transform;
            cardObj.GetComponent<Card>().ToggleSpirte(true);
            cardObj.GetComponent<Card>().ToggleButtonInteraction(false);
        }*/
    }    

    public void ResetDeck()
    {
        cardHolderPanel.KillAllChild();
        Initialize();
    }

    public void ShuffleDeck()
    {
        for (int i = suffledCard.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Card temp = suffledCard[i];
            suffledCard[i] = suffledCard[randomIndex];
            suffledCard[randomIndex] = temp;
        }
        DistributeCards();
    }

    public void DistributeCards()
    {
        StartCoroutine(FakeDistribute());
    }

    public IEnumerator FakeDistribute()
    {
        int cardsPerPlayer = suffledCard.Count / GameplayManager.Instance.Players().Count;
        for (int i = 0; i < GameplayManager.Instance.Players().Count; i++)
        {
            for (int j = 0; j < cardsPerPlayer; j++)
            {
                int index = i * cardsPerPlayer + j;
                GameplayManager.Instance.Players()[i].AddCard(usableCards[index]);
                yield return new WaitForSeconds(0.01f);
            }
        }

        yield return new WaitForSeconds(1f);
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < GameplayManager.Instance.Players().Count; i++)
            {
                List<int> playerCards = new List<int>();
                for (int j = 0; j < cardsPerPlayer; j++)
                {
                    int index = i * cardsPerPlayer + j;
                    playerCards.Add(suffledCard[index].CardNumber);
                }
                GameplayManager.Instance.Photon().PushCards(playerCards.ToArray(), GameplayManager.Instance.Players()[i].userId);
            }
        }
    }
}
