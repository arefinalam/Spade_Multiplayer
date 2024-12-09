using UnityEngine;

public class BetLevelPanelView : MonoBehaviour
{
    public GameObject betLevelViewPrefab;
    public Transform content;
    void Start()
    {
        GenerateBetLevelViews(GameManager.Instance.gameData);
    }

    public void GenerateBetLevelViews(GameData gameData)
    {
        foreach (var betLevel in gameData.betLevels)
        {
            GameObject betLevelViewGO = Instantiate(betLevelViewPrefab, content);
            BetLevelView betLevelView = betLevelViewGO.GetComponent<BetLevelView>();

            // Populate bet level view details
            betLevelViewGO.name = betLevel.name;
            betLevelView.SetBetLevelDetails(betLevel.name, betLevel.entryAmount, betLevel.betSprite);
        }
    }
}
