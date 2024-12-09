using UnityEngine;
using System.Collections.Generic;
using Photon.Realtime;

public class BidController : MonoBehaviour
{
    private Dictionary<PlayerController, int> playerBids;
    
    public void Initialize()
    {
        playerBids ??= new Dictionary<PlayerController, int>();
        playerBids.Clear();
    }
    
    public void StartBidPhase()
    {
        StartCoroutine(GameplayManager.Instance.GetSelfPlayer().ShowBidPopup());
    }
    public void CollectBid(PlayerController player, int bid)
    {
        playerBids[player] = bid;
        string message = $"Player {player.name} bid {bid}";
        LogManager.Instance.ConsoleLog(message);
        GameplayManager.Instance.UIManager().ReceiveGameplayMessage(message);
        if (playerBids.Count == GameplayManager.Instance.Players().Count)
        {
            message = "All bids collected.";
            LogManager.Instance.ConsoleLog(message);
            GameplayManager.Instance.UIManager().ReceiveGameplayMessage(message);
            LogManager.Instance.ConsoleLog("");
            
            PlayerController highestBidder = null;
            int highestBid = int.MinValue;

            foreach (var playerBid in playerBids)
            {
                if (playerBid.Value > highestBid)
                {
                    highestBid = playerBid.Value;
                    highestBidder = playerBid.Key;
                }
            }

            if (highestBidder != null)
            {
                message = ($"Highest bid by {highestBidder.userName} with {highestBid}");
                LogManager.Instance.ConsoleLog(message);
                GameplayManager.Instance.UIManager().ReceiveGameplayMessage(message);
                GameplayManager.Instance.Photon().PushGameStarter(highestBidder.tableIndex);
            }
        }
    }

    public Dictionary<PlayerController, int> GetPlayerBids()
    {
        return playerBids;
    }

    public void ResetBids()
    {
        playerBids.Clear();
    }
}
