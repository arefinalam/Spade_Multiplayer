using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public List<Transform> playerSpawnPositions;
    private List<PlayerController> players;
    
    public void SetPlayer(List<PlayerController> playerInRoom)
    {
        players ??= new List<PlayerController>();
        players.Clear();
        for (int i = 0; i < playerInRoom.Count; i++)
        {
            players.Add(playerInRoom[i]);
        }
    }

    public List<PlayerController> SetAllPlayers()
    {
        //set data for players
        /*Debug.Log($"size: {ControllerPhoton.self.playersInRoom.Count}/{players.Count}");
        for (int i = 0; i < ControllerPhoton.self.playersInRoom.Count; i++)
        {
            Debug.Log($"player info: {ControllerPhoton.self.playersInRoom[i].NickName} {ControllerPhoton.self.playersInRoom[i].UserId}");
            players[i].Setup(ControllerPhoton.self.playersInRoom[i].NickName, ControllerPhoton.self.playersInRoom[i].UserId);
        }*/

        // Set positions for players
        for (int i = 0; i < players.Count; i++)
        {
            players[i].transform.position = playerSpawnPositions[i].position;
            players[i].transform.rotation = playerSpawnPositions[i].rotation;
            players[i].tableIndex = i;
        }

        return players;
    }
}
