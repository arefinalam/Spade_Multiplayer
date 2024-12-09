using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LocalPlayerController : MonoBehaviour
{
    public List<Card> Hand; // List to hold the player's cards
    public TextMeshProUGUI playerStatusText; // UI element to display player status (e.g., turn status, bids)
    public Button playCardButton; // Button to play a selected card
    public Button bidButton; // Button to submit a bid
    public TMP_InputField bidInputField; // Input field to enter a bid amount

    private Card selectedCard; // The currently selected card

    private void Start()
    {
        playCardButton.onClick.AddListener(OnPlayCard);
        bidButton.onClick.AddListener(OnBid);
    }

    // Method to handle playing a card
    private void OnPlayCard()
    {
        if (selectedCard != null)
        {
            // Implement logic to play the selected card
            PlayCard(selectedCard);
        }
        else
        {
            playerStatusText.text = "No card selected.";
        }
    }

    // Method to handle bidding
    private void OnBid()
    {
        int bidAmount;
        if (int.TryParse(bidInputField.text, out bidAmount))
        {
            // Implement logic to submit the bid
            SubmitBid(bidAmount);
        }
        else
        {
            playerStatusText.text = "Invalid bid amount.";
        }
    }

    // Method to select a card (called when a card is clicked)
    public void SelectCard(Card card)
    {
        selectedCard = card;
        playerStatusText.text = "Selected card: " + card.name;
    }

    // Method to play a card (implement the game-specific logic here)
    private void PlayCard(Card card)
    {
        Hand.Remove(card);
        // Additional logic to play the card (e.g., update game state, notify server)
        playerStatusText.text = "Played card: " + card.name;
    }

    // Method to submit a bid (implement the game-specific logic here)
    private void SubmitBid(int bidAmount)
    {
        // Additional logic to submit the bid (e.g., update game state, notify server)
        playerStatusText.text = "Bid submitted: " + bidAmount;
    }

    // Method to add a card to the player's hand (e.g., during dealing)
    public void AddCardToHand(Card card)
    {
        Hand.Add(card);
        // Update the UI or player state as needed
    }

    // Method to handle player's turn (e.g., enable/disable UI elements)
    public void OnPlayerTurn()
    {
        playerStatusText.text = "Your turn!";
        playCardButton.interactable = true;
        bidButton.interactable = true;
        bidInputField.interactable = true;
    }

    // Method to handle end of player's turn
    public void OnEndPlayerTurn()
    {
        playCardButton.interactable = false;
        bidButton.interactable = false;
        bidInputField.interactable = false;
        playerStatusText.text = "Waiting for other players...";
    }
}
