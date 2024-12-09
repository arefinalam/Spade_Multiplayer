using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SandboxCalculation : MonoBehaviour
{
    private Dictionary<SandboxPlayerData, int> playerScores; // Player scores
    private Dictionary<SandboxPlayerData, int> playerBags; // Player bags count

    private bool nextRoundForTie = false;

    // Calculate player score based on bid and tricks won
    public SandboxGameplay.ScoreDataJson GetPlayerScore(Dictionary<SandboxPlayerData, int> playerBids, Dictionary<SandboxPlayerData, int> trickWinners)
    {
        foreach (var item in playerBids)
        {
            Debug.Log($"bid: {item.Key.username} {item.Value}");
        }

        foreach (var item in trickWinners)
        {
            Debug.Log($"trick: {item.Key.username} {item.Value}");
        }

        playerScores ??= new Dictionary<SandboxPlayerData, int>(); // Player scores
        playerScores.Clear();
        playerBags ??= new Dictionary<SandboxPlayerData, int>(); // Player bags count
        playerBags.Clear();

        nextRoundForTie = false;

        long winBidMultiplier = SandboxGameplay.self.so.winBidMultiplier;
        long lowerBidMultiplier = SandboxGameplay.self.so.lowerBidMultiplier;
        long nillBidSuccessPoint = SandboxGameplay.self.so.nillBidSuccessPoint;
        long nillBidFailedPenalty = SandboxGameplay.self.so.nillBidFailedPenalty;
        int overFlowAdditionalPoint = SandboxGameplay.self.so.overFlowAdditionalPoint;
        int maxOverFlowPoint = SandboxGameplay.self.so.maxOverFlowPoint;
        long overFlowPointPenalty = SandboxGameplay.self.so.overFlowPointPenalty;

        Dictionary<SandboxPlayerData, int> scores = new Dictionary<SandboxPlayerData, int>();

        foreach (var playerBid in playerBids)
        {
            SandboxPlayerData player = playerBid.Key;
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
            if (tricksWon > bid)
            {
                bags += tricksWon - bid;
                SetPlayerBags(player, bags);
            }

            // Apply penalty for exceeding max overtricks
            if (bags >= maxOverFlowPoint)
            {
                score -= (int)overFlowPointPenalty;
                // Reset bags count
                SetPlayerBags(player, 0);
            }

            scores[player] = score;

            // Update the player's total score
            SetPlayerScore(player, score);
        }


        // sort data for ranking
        var _t = playerScores.OrderByDescending(entry => entry.Value).ToDictionary(entry => entry.Key, entry => entry.Value);

        SandboxGameplay.ScoreDataJson scoreDataJson = new SandboxGameplay.ScoreDataJson();
        foreach (var item in _t)
        {
            string username = item.Key.username;
            int score = GetPlayerScore(item.Key);
            int bag = GetPlayerBags(item.Key);
            SandboxGameplay.ScoreData data = new SandboxGameplay.ScoreData(username, score, bag);
            scoreDataJson.scoreData.Add(data);

            Debug.Log($"Result: {username} {score} {bag}");
        }
        return scoreDataJson;
    }


    // Get player score
    int GetPlayerScore(SandboxPlayerData player) => playerScores.ContainsKey(player) ? playerScores[player] : 0;

    // Get player bags count
    int GetPlayerBags(SandboxPlayerData player) => playerBags.ContainsKey(player) ? playerBags[player] : 0;


    // Add or update player score
    void SetPlayerScore(SandboxPlayerData player, int score)
    {
        if (playerScores.ContainsKey(player)) playerScores[player] += score;
        else playerScores.Add(player, score);
    }

    // Add or update player bags count
    void SetPlayerBags(SandboxPlayerData player, int bags)
    {
        if (playerBags.ContainsKey(player)) playerBags[player] = bags;
        else playerBags.Add(player, bags);
    }

    public SandboxGameplay.ScoreDataJson CheckForWinner()
    {
        var _t = CheckForWinnerDict();

        if(_t == null) return null;

        SandboxGameplay.ScoreDataJson scoreDataJson = new SandboxGameplay.ScoreDataJson();
        foreach (var item in _t)
        {
            string username = item.Key.username;
            int score = GetPlayerScore(item.Key);
            int bag = GetPlayerBags(item.Key);
            SandboxGameplay.ScoreData data = new SandboxGameplay.ScoreData(username, score, bag);
            scoreDataJson.scoreData.Add(data);
        }

        return scoreDataJson;
    }

    Dictionary<SandboxPlayerData, int> CheckForWinnerDict()
    {
        long winningScore = GameManager.Instance.gameData.winningPoint;

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
            int highestScore = potentialWinners.Max(entry => entry.Value);
            var highestScorers = potentialWinners.Where(entry => entry.Value == highestScore).ToList();

            if (highestScorers.Count == 1)
            {
                // One player has the highest score
                return highestScorers.ToDictionary(entry => entry.Key, entry => entry.Value);
            }
            else
            {
                // Multiple players have the highest score, proceed to tie-breaker round
                nextRoundForTie = true;
                return null;
            }
        }
    }

    bool IsTieBreakerRound()
    {
        return nextRoundForTie;
    }

    void ResetTieBreakerFlag()
    {
        nextRoundForTie = false;
    }
}
