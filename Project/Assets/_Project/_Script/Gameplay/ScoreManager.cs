using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class ScoreManager : MonoBehaviour
{
    private Dictionary<PlayerController, int> playerScores; // Player scores
    private Dictionary<PlayerController, int> playerBags; // Player bags count

    private bool nextRoundForTie = false;
    public void Initialize()
    {
        playerScores ??= new Dictionary<PlayerController, int>(); // Player scores
        playerScores.Clear();
        playerBags ??= new Dictionary<PlayerController, int>(); // Player bags count
        playerBags.Clear();

        nextRoundForTie = false;
    }


    // Add or update player score
    void SetPlayerScore(PlayerController player, int score)
    {
        if (playerScores.ContainsKey(player))
        {
            playerScores[player] += score;
        }
        else
        {
            playerScores.Add(player, score);
        }
        GameplayManager.Instance.Photon().PushResult(player, playerScores[player]);
    }

    // Get player score
    public int GetPlayerScore(PlayerController player)
    {
        if (playerScores.ContainsKey(player))
        {
            return playerScores[player];
        }
        else
        {
            return 0; // Default score
        }
    }

    // Add or update player bags count
    void SetPlayerBags(PlayerController player, int bags)
    {
        if (playerBags.ContainsKey(player))
        {
            playerBags[player] = bags;
        }
        else
        {
            playerBags.Add(player, bags);
        }
    }

    // Get player bags count
    public int GetPlayerBags(PlayerController player)
    {
        if (playerBags.ContainsKey(player))
        {
            return playerBags[player];
        }
        else
        {
            return 0; // Default bags count
        }
    }

    // Calculate player score based on bid and tricks won
    public void CalculatePlayerScore(Dictionary<PlayerController, int> playerBids, Dictionary<PlayerController, int> trickWinners)
    {
        long winBidMultiplier = GameplayManager.Instance.gameData.winBidMultiplier;
        long lowerBidMultiplier = GameplayManager.Instance.gameData.lowerBidMultiplier;
        long nillBidSuccessPoint = GameplayManager.Instance.gameData.nillBidSuccessPoint;
        long nillBidFailedPenalty = GameplayManager.Instance.gameData.nillBidFailedPenalty;
        int overFlowAdditionalPoint = GameplayManager.Instance.gameData.overFlowAdditionalPoint;
        int maxOverFlowPoint = GameplayManager.Instance.gameData.maxOverFlowPoint;
        long overFlowPointPenalty = GameplayManager.Instance.gameData.overFlowPointPenalty;

        Dictionary<PlayerController, int> scores = new Dictionary<PlayerController, int>();

        foreach (var playerBid in playerBids)
        {
            PlayerController player = playerBid.Key;
            int bid = playerBid.Value;
            int tricksWon = trickWinners.ContainsKey(player) ? trickWinners[player] : 0;

            int score = 0;

            // Check if the player met their bid
            if (bid > 0)
            {
                if (tricksWon == bid)
                {
                    score += (int)(bid * winBidMultiplier);
                }
                else
                {
                    score -= (int)(Math.Abs(bid - tricksWon) * lowerBidMultiplier);
                }
            }

            // Check for Nil bid
            if (bid == 0)
            {
                if (tricksWon == 0)
                {
                    score += (int)nillBidSuccessPoint;
                }
                else
                {
                    score -= (int)nillBidFailedPenalty;
                }
            }

            // Calculate additional points for overtricks
            if (tricksWon > bid)
            {
                score += (tricksWon - bid) * overFlowAdditionalPoint;
            }

            // Update bags count
            int bags = GetPlayerBags(player);
            bags += tricksWon - bid;
            SetPlayerBags(player, bags);

            // Apply penalty for exceeding max overtricks
            if (bags >= maxOverFlowPoint)
            {
                score -= (int)overFlowPointPenalty;
                // Reset bags count
                SetPlayerBags(player, 0);
            }

            if(score < 0)
            {
                score = 0;
            }

            scores[player] = score;

            // Update the player's total score
            SetPlayerScore(player, score);
        }
    }

    public Dictionary<PlayerController, int> CheckForWinner()
    {
        long winningScore = GameplayManager.Instance.gameData.winningPoint;

        // Check if any player has reached or exceeded the winning score
        var potentialWinners = playerScores.Where(entry => entry.Value >= winningScore).ToList();

        if (potentialWinners.Count == 0)
        {
            // No winner yet
            return null;
        }
        else if (potentialWinners.Count == 1)
        {
            // Only one player has reached the winning score
            return potentialWinners.ToDictionary(entry => entry.Key, entry => entry.Value);
        }
        else
        {
            // Multiple players have reached the winning score
            // Continue to next round for tie-breaker
            nextRoundForTie = true;
            return null;
        }
    }

    public bool IsTieBreakerRound()
    {
        return nextRoundForTie;
    }

    public void ResetTieBreakerFlag()
    {
        nextRoundForTie = false;
    }

    public Dictionary<PlayerController, int> GetFinalScores()
    {
        // Sort players by their scores in descending order
        Dictionary<PlayerController, int> sortedPlayers = playerScores.OrderByDescending(entry => entry.Value)
                                                                      .ToDictionary(entry => entry.Key, entry => entry.Value);

        return sortedPlayers;
    }
}
