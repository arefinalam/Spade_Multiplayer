using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameData", menuName = "Game/GameData")]
public class GameData : ScriptableObject
{
    [Serializable]
    public class BetLevel
    {
        public string name;
        public int entryAmount;
        public Sprite betSprite;
    }
    [Serializable]
    public class DepositStore
    {
        public int price;
        public int currencyAmount;
    }

    public GameMode currentMode;

    [Header("Currency Convert")]
    public long coinPerUSD = 10;

    [Header("Bet Levels")]
    public List<BetLevel> betLevels;
    public List<DepositStore> depositStore;

    [Header("Room Settings")]
    public int maxPlayersInRoom;

    [Header("Board Settings")]
    public float boardCommision = 0.25f;
    public float startingWaitTime = 10f;
    public float phaseWaitTime = 3f;

    public float eachTurnDuration = 10f;
    public float turnAlertDelay = 7f;
    public float delayBeforeNewTurn = 3f;
    public float bidPopupDelay = 10f;
    public float maxBidTime = 30f;

    public float newGameStartDelay = 10f;
    public float winnerPanelTime = 10f;

    public long winningPoint = 500;
    public long winBidMultiplier = 10;
    public long lowerBidMultiplier = 10;
    public long nillBidSuccessPoint = 50;
    public long nillBidFailedPenalty = 50;
    public int overFlowAdditionalPoint = 1;
    public int maxOverFlowPoint = 5;
    public long overFlowPointPenalty = 50;
}
