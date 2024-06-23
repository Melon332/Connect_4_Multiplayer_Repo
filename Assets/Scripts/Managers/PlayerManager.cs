using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : NetworkBehaviour
{
    public int PlayerID { get; set; }
    public bool IsMyTurn { get; set; }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        PlayerID = (int)OwnerClientId;
        if (PlayerID == 0)
        {
            IsMyTurn = true;
        }
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManagerOnLoadEventCompleted;
    }

    private void SceneManagerOnLoadEventCompleted(string scenename, LoadSceneMode loadscenemode, List<ulong> clientscompleted, List<ulong> clientstimedout)
    {
        if (scenename == "Gameplay")
        {
            GameManager.Instance.AddPlayer(this);
        }
    }

    private void Update()
    {
        if (!IsOwner) return;
        if (Input.GetKeyDown(KeyCode.Return))
        {
            UIManager.Instance.ToggleChatBox();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.Instance.TestPrintBoard();
        }
        
        ProcessInput();
    }

    private void ProcessInput()
    {
        if (!IsMyTurn) return;
        Action<int> action = IsHost == true ? ServerSetTileOnColumnRpc : ClientSetTileOnColumnRpc;
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            action.Invoke(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            action.Invoke(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            action.Invoke(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            action.Invoke(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            action.Invoke(4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            action.Invoke(5);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            action.Invoke(6);
        }
    }

    [Rpc(SendTo.Server)]
    private void ClientSetTileOnColumnRpc(int column)
    {
        ServerSetTileOnColumnRpc(column);
    }

    [Rpc(SendTo.Everyone)]
    private void ServerSetTileOnColumnRpc(int column)
    {
        GameManager.Instance.SetTile(column, PlayerID + 1);
    }

    private void OnDisable()
    {
        
    }
}
