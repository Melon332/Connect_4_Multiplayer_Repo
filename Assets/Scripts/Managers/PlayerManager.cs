using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using Unity.Netcode;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    public int PlayerID { get; set; }
    
    private void Update()
    {
        if (!IsOwner) return;
        if (Input.GetKeyDown(KeyCode.Return))
        {
            UIManager.Instance.ToggleChatBox();
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (IsHost)
            {
                StartGameRpc();
            }
            else
            {
                Debug.LogError($"{SteamClient.Name} tried to start the game as non server!");
            }
        }
    }

    [Rpc(SendTo.Everyone)]
    private void StartGameRpc()
    {
        GameManager.Instance.StartGame();
    }
}
