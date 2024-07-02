using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Steamworks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : NetworkBehaviour
{
    public Camera cameraMain { private get; set; }
    public int PlayerID { get; private set; }
    public bool IsMyTurn { get; set; }
    
    public string PlayerName { get; set; }
    
    public bool IsPaused { get; set; }
    
    public int Wins { get; set; }

    private bool inGame = false;

    private RowNumber currentHoveringRow;

    private Action<int> SetTileAction;

    private CharacterContainer ownedCharacterContainer;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        PlayerID = (int)OwnerClientId;
        if (PlayerID == 0)
        {
            IsMyTurn = true;
        }
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManagerOnLoadEventCompleted;
        IsPaused = false;
        SetTileAction += IsHost ? ServerSetTileOnColumnRpc : ClientSetTileOnColumnRpc;
        ownedCharacterContainer = CharacterCustomizationManager.Instance.GetCharacterContainer();
        if (!NetworkManager.Singleton.IsHost && IsOwner)
        {
            ClientJoinedRpc();
            SendCustomizationDataToServerRpc(ownedCharacterContainer.currentTorso,
                ownedCharacterContainer.currentHead, ownedCharacterContainer.currentEars, ownedCharacterContainer.currentEyes);
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SceneManagerOnLoadEventCompleted;
    }

    private void SceneManagerOnLoadEventCompleted(string scenename, LoadSceneMode loadscenemode, List<ulong> clientscompleted, List<ulong> clientstimedout)
    {
        if (scenename == "Gameplay")
        {
            GameManager.Instance.AddPlayer(this);
            PlayerName = LobbySaver.CurrentLobby.Members.ToList()[PlayerID].Name;
            inGame = true;
            cameraMain = FindObjectOfType<Camera>();
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
            GameManager.Instance?.TestPrintBoard();
        }
        
        ProcessInput();
    }

    private void ProcessInput()
    {
        if (!inGame) return;
        if (Input.GetKeyDown(KeyCode.P))
        {
            GameManager.Instance.PauseGame(!IsPaused);
        }
        HoveringOverRow();
        if (!IsMyTurn || IsPaused || GameManager.Instance.IsDroppingTile) return;
        MouseClick();
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetTileAction.Invoke(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetTileAction.Invoke(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetTileAction.Invoke(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SetTileAction.Invoke(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SetTileAction.Invoke(4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SetTileAction.Invoke(5);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            SetTileAction.Invoke(6);
        }
    }

    [Rpc(SendTo.Server)]
    private void ClientJoinedRpc()
    {
        UIManager.Instance.ToggleStartGameButton();
    }

    [Rpc(SendTo.Server)]
    private void SendCustomizationDataToServerRpc(int torso, int head, int ears, int eyes)
    {
        CharacterCustomizationManager.Instance.AddNewCustomization(torso, head, ears, eyes);
        SendCustomizationDataToClientsRpc(ownedCharacterContainer.currentTorso,
            ownedCharacterContainer.currentHead,
            ownedCharacterContainer.currentEars,
            ownedCharacterContainer.currentEyes);
    }

    [Rpc(SendTo.NotServer)]
    private void SendCustomizationDataToClientsRpc(int torso, int head, int ears, int eyes)
    {
        CharacterCustomizationManager.Instance.AddNewCustomization(torso,
            head,
            ears,
            eyes);
    }

    private void HoveringOverRow()
    {
        Ray ray = cameraMain.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            RowNumber hoveringRow = hit.collider.GetComponent<RowNumber>();
            if (hoveringRow)
            {
                currentHoveringRow = hoveringRow;
                GameManager.Instance.SetPhantomTilePosition(PlayerID, hoveringRow.transform);
            }
        }
        else
        {
            currentHoveringRow = null;
            GameManager.Instance.TogglePhantomTile(PlayerID, false);
        }
    }

    private void MouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (currentHoveringRow)
            {
                SetTileAction?.Invoke(currentHoveringRow.number);
            }
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
}
