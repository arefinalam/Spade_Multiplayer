using UnityEngine;
using System.Collections.Generic;

public class RoundManager : MonoBehaviour
{
    private int currentPlayerIndex;
    public void Initialize()
    {
        currentPlayerIndex = 0;
    }

    public void SetHighestBidder(PlayerController player)
    {
        currentPlayerIndex = player.tableIndex;
    }

    public void StartTrickPlaying()
    {
        //GameplayManager.Instance.Photon().PushNextPlayerTurn();
    }

    public void NextPlayerToPlay()
    {                
        foreach (PlayerController player in GameplayManager.Instance.Players())
        {
            player.PlayTrick(currentPlayerIndex);
        }
        currentPlayerIndex = (currentPlayerIndex+1) % GameplayManager.Instance.Players().Count;
    }

    public void SetTrickWinner(PlayerController player)
    {
        currentPlayerIndex = player.tableIndex;
    }

    public void EndBidRound(Dictionary<PlayerController, int> trickWinners)
    {
        GameplayManager.Instance.ScoreManager().CalculatePlayerScore(GameplayManager.Instance.BidController().GetPlayerBids(),
            trickWinners);

        CheckGameEnd();
    }

    public void CheckGameEnd()
    {
        Dictionary<PlayerController, int> winnerList =  GameplayManager.Instance.ScoreManager().CheckForWinner();
        if (winnerList == null)
        {
            if (GameplayManager.Instance.ScoreManager().IsTieBreakerRound())
            {
                GameplayManager.Instance.ScoreManager().ResetTieBreakerFlag();
            }
            StartNextRound();
        }
        else
        {
            GameplayManager.Instance.UpdateStep();
        }
    }

    public void StartNextRound()
    {
        GameplayManager.Instance.BidController().ResetBids();
        GameplayManager.Instance.TrickManager().ResetAll();
        ResetPlayerData();
        GameplayManager.Instance.StartSuffleAndDistributeCards();
    }

    private void ResetPlayerData()
    {
        foreach(PlayerController player in GameplayManager.Instance.Players())
        {
            player.ResetData();
        }
    }

}
