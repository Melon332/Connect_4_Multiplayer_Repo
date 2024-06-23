using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using Unity.Netcode;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    public int PlayerID { get; set; }
    public bool IsMyTurn { get; set; }
    
    private void Update()
    {
        if (!IsOwner) return;
        if (Input.GetKeyDown(KeyCode.Return))
        {
            UIManager.Instance.ToggleChatBox();
        }
    }

    [Rpc(SendTo.Everyone)]
    private void StartGameRpc()
    {
        GameManager.Instance.StartGame();
    }
}
