using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrickManager : MonoBehaviour
{
    public Transform deckParentTransform;
    public Transform cardHolderParent;
    private Dictionary<PlayerController, Card> cardsOnDeck;
    private Dictionary<PlayerController, int> trickWinners;

    private int totalTrickCount;

    public List<Card> allPlayedCards;
    public void Initialize()
    {
        cardsOnDeck ??= new Dictionary<PlayerController, Card>();
        cardsOnDeck.Clear();
        trickWinners ??= new Dictionary<PlayerController, int>();
        trickWinners.Clear();

        totalTrickCount = 0;
        allPlayedCards ??= new List<Card>();
        allPlayedCards.Clear();
        ResetDeck();
    }
    public void ResetTrickCounter()
    {
        totalTrickCount = 0;
    }
    private void ResetDeck()
    {
        cardsOnDeck.Clear();
        DisableDechCards();
    }
    private void DisableDechCards()
    {
        foreach (Transform card in cardHolderParent.transform)
        {
            card.gameObject.SetActive(false);
        }
    }

    public void ResetAll()
    {
        cardsOnDeck = new Dictionary<PlayerController, Card>();
        trickWinners = new Dictionary<PlayerController, int>();

        totalTrickCount = 0;
        //GameplayManager.Instance.CardManager().ResetDeck(allPlayedCards);
        allPlayedCards.Clear();
        ResetDeck();
    }
    // Coroutine to play tricks
    public void TrickPlayed(PlayerController player, Card card)
    {
        cardsOnDeck[player] = card;
        allPlayedCards.Add(card);
        LogManager.Instance.ConsoleLog($"Player {player.userName} played {card.Rank} {card.Suit}");

        if (cardsOnDeck.Count == GameplayManager.Instance.Players().Count)
        {
            LogManager.Instance.ConsoleLog("All tricks collected.");
            DisableDechCards();
            // Proceed to the next phase
            StartCoroutine(ResolveTrick());
            
            if(totalTrickCount >= 13)
            {
                GameplayManager.Instance.RoundManager().EndBidRound(trickWinners);
            }
            else
            {
                NextPlayerTurn();
            }
        }
        else
        {
            NextPlayerTurn();
        }
    }

    public int GetTotalTrickCount()
    {
        return totalTrickCount;
    }

    // Method to resolve a trick and determine the winner
    private IEnumerator ResolveTrick()
    {
        totalTrickCount++;
        // Get the trick winner
        PlayerController winner = GetTrickWinner();

        // Update the trick winner counter
        if (trickWinners.ContainsKey(winner))
        {
            trickWinners[winner]++;
        }
        else
        {
            trickWinners[winner] = 1;
        }

        // Log the winner for debugging purposes
        GameplayManager.Instance.Photon().PushTrickWinner(winner.userId);
        string message = ($"Player {winner.userName} wins the trick!");
        LogManager.Instance.ConsoleLog(message);
        GameplayManager.Instance.UIManager().ReceiveGameplayMessage(message);
       
        // Clear the cards on deck for the next trick
        ResetDeck();

        // Add a delay for visual purposes (optional)
        yield return new WaitForSeconds(1.0f);
    }
    // Method to get the current trick winner by comparing the cards on the deck
    private PlayerController GetTrickWinner()
    {
        PlayerController leadingPlayer = null;
        Card leadingCard = null;

        foreach (var entry in cardsOnDeck)
        {
            PlayerController player = entry.Key;
            Card card = entry.Value;

            if (leadingCard == null)
            {
                leadingCard = card;
                leadingPlayer = player;
            }
            else
            {
                if (card.Suit == leadingCard.Suit)
                {
                    if (card.Rank > leadingCard.Rank)
                    {
                        leadingCard = card;
                        leadingPlayer = player;
                    }
                }
                else if (card.Suit == Suit.Spades)
                {
                    leadingCard = card;
                    leadingPlayer = player;
                }
            }
        }

        return leadingPlayer;
    }

    public void SetPlayerTrickStats()
    {
        foreach(var entry in trickWinners)
        {
            PlayerController player = entry.Key;
            int totalTrickWOn = entry.Value;
            player.SetTotalTrickWon(totalTrickWOn);
        }
    }

    // Method to update player turn order for the next trick
    private void NextPlayerTurn()
    {
        GameplayManager.Instance.Photon().NextPlayerTurn();
    }

    public Card CardsEligibleToPlay()
    {
        if (cardsOnDeck.Count == 0)
        {
            return null;
        }
        
        return cardsOnDeck.Values.First();
    }
}
